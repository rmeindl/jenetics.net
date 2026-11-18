// Java Genetic Algorithm Library.
// Copyright (c) 2017 Franz Wilhelmstötter
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using static Jenetics.Internal.Util.Require;

namespace Jenetics.Stat;

public sealed class Quantile
{
    private readonly double _quantile;
    private readonly double[] _heights = new double[5];
    private readonly double[] _positions = new double[5];
    private readonly double[] _desiredPositions = new double[3];
    private readonly double[] _positionIncrements = new double[3];
    private readonly double[] _initialValues = new double[5];

    private long _samples;
    private int _initialCount;
    private bool _initialized;

    public Quantile(double quantile)
    {
        if (double.IsNaN(quantile) || quantile < 0.0 || quantile > 1.0)
            throw new ArgumentOutOfRangeException(nameof(quantile), "Quantile must be in [0, 1].");

        _quantile = quantile;
    }

    public double QuantileValue => _quantile;

    public double Value
    {
        get
        {
            if (_samples == 0)
                return double.NaN;

            if (_quantile == 0.0 || _quantile == 1.0 || _initialized)
                return _heights[2];

            var sorted = new double[_initialCount];
            Array.Copy(_initialValues, sorted, _initialCount);
            Array.Sort(sorted);

            var position = (sorted.Length - 1) * _quantile;
            var lower = (int)Math.Floor(position);
            var upper = (int)Math.Ceiling(position);
            return sorted[lower] + (sorted[upper] - sorted[lower]) * (position - lower);
        }
    }

    public long Samples => _samples;

    public void Reset()
    {
        Array.Clear(_heights);
        Array.Clear(_positions);
        Array.Clear(_desiredPositions);
        Array.Clear(_positionIncrements);
        Array.Clear(_initialValues);
        _samples = 0;
        _initialCount = 0;
        _initialized = false;
    }

    public void Accept(double value)
    {
        if (_quantile == 0.0 || _quantile == 1.0)
        {
            if (_samples == 0)
                _heights[2] = value;
            else if (_quantile == 0.0)
                _heights[2] = Math.Min(_heights[2], value);
            else
                _heights[2] = Math.Max(_heights[2], value);

            ++_samples;
            return;
        }

        if (!_initialized)
        {
            _initialValues[_initialCount++] = value;
            ++_samples;
            if (_initialCount == _initialValues.Length)
                InitializeMarkers();
        }
        else
        {
            UpdateMarkers(value);
            ++_samples;
        }
    }

    public Quantile Combine(Quantile other)
    {
        NonNull(other);
        if (_quantile != other._quantile)
            throw new ArgumentException("Quantiles must be equal to combine.", nameof(other));
        if (other._samples == 0)
            return this;
        if (_samples == 0)
        {
            CopyState(other);
            return this;
        }

        if (_quantile == 0.0 || _quantile == 1.0)
        {
            _heights[2] = _quantile == 0.0
                ? Math.Min(_heights[2], other._heights[2])
                : Math.Max(_heights[2], other._heights[2]);
            _samples += other._samples;
            return this;
        }

        if (!_initialized && !other._initialized)
        {
            var otherInitialCount = other._initialCount;
            var otherInitialValues = new double[otherInitialCount];
            Array.Copy(other._initialValues, otherInitialValues, otherInitialCount);
            for (var i = 0; i < otherInitialCount; ++i)
                Accept(otherInitialValues[i]);
            return this;
        }

        if (_initialized && !other._initialized)
        {
            for (var i = 0; i < other._initialCount; ++i)
                Accept(other._initialValues[i]);
            return this;
        }

        if (!_initialized)
        {
            var initialCount = _initialCount;
            var initialValues = new double[initialCount];
            Array.Copy(_initialValues, initialValues, initialCount);
            CopyState(other);
            for (var i = 0; i < initialCount; ++i)
                Accept(initialValues[i]);
            return this;
        }

        _samples += other._samples;
        for (var i = 1; i < 5; ++i)
            _positions[i] += other._positions[i];

        _heights[0] = Math.Min(_heights[0], other._heights[0]);
        for (var i = 1; i < 4; ++i)
            _heights[i] = (_heights[i] + other._heights[i]) * 0.5;
        _heights[4] = Math.Max(_heights[4], other._heights[4]);

        for (var i = 0; i < 3; ++i)
            _desiredPositions[i] += other._desiredPositions[i];

        AdjustMarkerHeights();
        return this;
    }

    public bool SameState(Quantile other)
    {
        NonNull(other);
        return _quantile == other._quantile &&
               _samples == other._samples &&
               _initialCount == other._initialCount &&
               _initialized == other._initialized &&
               _heights.AsSpan().SequenceEqual(other._heights) &&
               _positions.AsSpan().SequenceEqual(other._positions) &&
               _desiredPositions.AsSpan().SequenceEqual(other._desiredPositions) &&
               _positionIncrements.AsSpan().SequenceEqual(other._positionIncrements) &&
               _initialValues.AsSpan().SequenceEqual(other._initialValues);
    }

    public static Quantile ToQuantile<T>(
        IEnumerable<T> values,
        double quantile,
        Func<T, double> mapper
    )
    {
        NonNull(values);
        NonNull(mapper);
        var result = new Quantile(quantile);
        foreach (var value in values)
            result.Accept(mapper(value));
        return result;
    }

    public static Quantile ToMedian<T>(IEnumerable<T> values, Func<T, double> mapper)
    {
        return ToQuantile(values, 0.5, mapper);
    }

    private void InitializeMarkers()
    {
        Array.Copy(_initialValues, _heights, _heights.Length);
        Array.Sort(_heights);
        for (var i = 0; i < _positions.Length; ++i)
            _positions[i] = i;

        _desiredPositions[0] = 2.0 * _quantile;
        _desiredPositions[1] = 4.0 * _quantile;
        _desiredPositions[2] = 2.0 * _quantile + 2.0;
        _positionIncrements[0] = _quantile / 2.0;
        _positionIncrements[1] = _quantile;
        _positionIncrements[2] = (1.0 + _quantile) / 2.0;
        _initialized = true;
    }

    private void UpdateMarkers(double value)
    {
        if (value < _heights[0])
        {
            for (var i = 1; i < _positions.Length; ++i)
                ++_positions[i];
            _heights[0] = value;
        }
        else if (value < _heights[1])
        {
            for (var i = 1; i < _positions.Length; ++i)
                ++_positions[i];
        }
        else if (value < _heights[2])
        {
            for (var i = 2; i < _positions.Length; ++i)
                ++_positions[i];
        }
        else if (value < _heights[3])
        {
            for (var i = 3; i < _positions.Length; ++i)
                ++_positions[i];
        }
        else if (value < _heights[4])
        {
            ++_positions[4];
        }
        else
        {
            ++_positions[4];
            _heights[4] = value;
        }

        for (var i = 0; i < _desiredPositions.Length; ++i)
            _desiredPositions[i] += _positionIncrements[i];

        AdjustMarkerHeights();
    }

    private void AdjustMarkerHeights()
    {
        for (var i = 1; i < 4; ++i)
        {
            var lower = _positions[i] - 1.0;
            var upper = _positions[i] + 1.0;
            var direction = _desiredPositions[i - 1] >= upper && _positions[i + 1] > upper
                ? 1
                : _desiredPositions[i - 1] <= lower && _positions[i - 1] < lower
                    ? -1
                    : 0;

            if (direction == 0)
                continue;

            var candidate = ParabolicEstimate(i, direction);
            if (_heights[i - 1] < candidate && candidate < _heights[i + 1])
                _heights[i] = candidate;
            else
                _heights[i] = LinearEstimate(i, direction);

            _positions[i] += direction;
        }
    }

    private double ParabolicEstimate(int index, int direction)
    {
        var p0 = index - 1;
        var p1 = index;
        var p2 = index + 1;
        return _heights[p1] +
               direction / (_positions[p2] - _positions[p0]) *
               ((_positions[p1] - _positions[p0] + direction) *
                (_heights[p2] - _heights[p1]) / (_positions[p2] - _positions[p1]) +
                (_positions[p2] - _positions[p1] - direction) *
                (_heights[p1] - _heights[p0]) / (_positions[p1] - _positions[p0]));
    }

    private double LinearEstimate(int index, int direction)
    {
        var neighbor = index + direction;
        return _heights[index] +
               direction * (_heights[neighbor] - _heights[index]) /
               (_positions[neighbor] - _positions[index]);
    }

    private void CopyState(Quantile other)
    {
        _samples = other._samples;
        _initialCount = other._initialCount;
        _initialized = other._initialized;
        Array.Copy(other._heights, _heights, _heights.Length);
        Array.Copy(other._positions, _positions, _positions.Length);
        Array.Copy(other._desiredPositions, _desiredPositions, _desiredPositions.Length);
        Array.Copy(other._positionIncrements, _positionIncrements, _positionIncrements.Length);
        Array.Copy(other._initialValues, _initialValues, _initialValues.Length);
    }
}
