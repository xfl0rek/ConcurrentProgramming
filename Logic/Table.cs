﻿using System.Numerics;
using Data;

namespace Logic;

internal class Table : LogicApi, IObserver<DataApi>, IObservable<LogicApi>
{
    private readonly object _ballsLock = new();
    private readonly int _height;
    private readonly List<IObserver<LogicApi>> _observers = [];
    private readonly int _width;
    private IDisposable? _subscriptionToken;

    public Table(int width, int height)
    {
        _width = width;
        _height = height;
        lock (_ballsLock)
        {
            Balls = [];
        }

        _observers = [];
    }

    public Table(int width, int height, List<DataApi> balls)
    {
        _width = width;
        _height = height;
        lock (_ballsLock)
        {
            Balls = balls;
        }
    }

    public override int Width => _width;

    public override int Height => _height;

    private List<DataApi> Balls { get; }

    public override IDisposable Subscribe(IObserver<LogicApi> observer)
    {
        if (!_observers.Contains(observer)) _observers.Add(observer);
        return new SubscriptionToken(_observers, observer);
    }

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(DataApi value)
    {
        WallCollision(value);

        lock (_ballsLock)
        {
            foreach (var ball1 in Balls)
                if (ball1 != value)
                    BallCollision(ball1, value);
        }

        NotifyObservers(this);
    }

    public override List<List<float>> GetBallPositions()
    {
        var ballPositions = new List<List<float>>();
        lock (_ballsLock)
        {
            foreach (var ball in Balls)
            {
                var ballPosition = new List<float> { ball.Position.X, ball.Position.Y };
                ballPositions.Add(ballPosition);
            }
        }

        return ballPositions;
    }

    public override void Start(int number, int radius, float velocity)
    {
        var random = new Random();

        lock (_ballsLock)
        {
            for (var i = 0; i < number; i++)
            {
                Vector2 position;
                bool overlaps;
                do
                {
                    float x = random.Next(0 + radius, _width - radius);
                    float y = random.Next(0 + radius, _height - radius);
                    position = new Vector2(x, y);

                    overlaps = Balls.Any(existingBall =>
                        Vector2.Distance(existingBall.Position, position) < existingBall.Radius + radius);
                } while (overlaps);

                var ball = DataApi.Instance(position, radius, velocity, random);
                Balls.Add(ball);
                Subscribe(ball);
            }
        }
    }

    public override void ResetTable()
    {
        lock (_ballsLock)
        {
            foreach (var ball in Balls) ball.IsStopped = true;
            Balls.Clear();
        }

        lock (_ballsLock)
        {
            foreach (var _ in Balls) Unsubscribe();
        }
    }

    private void Subscribe(DataApi provider)
    {
        _subscriptionToken = provider.Subscribe(this);
    }

    public void Unsubscribe()
    {
        _subscriptionToken?.Dispose();
    }

    private void WallCollision(DataApi ball)
    {
        var position = ball.Position;
        var velocity = ball.Velocity;
        var radius = ball.Radius;

        if (position.X - radius < 0)
        {
            if (velocity.X < 0)
                velocity.X = Math.Abs(velocity.X);
        }
        else if (position.X + radius > _width)
        {
            if (velocity.X > 0)
                velocity.X = -Math.Abs(velocity.X);
        }

        if (position.Y - radius < 0)
        {
            if (velocity.Y < 0)
                velocity.Y = Math.Abs(velocity.Y);
        }
        else if (position.Y + radius > _height)
        {
            if (velocity.Y > 0)
                velocity.Y = -Math.Abs(velocity.Y);
        }

        ball.Velocity = velocity;
    }

    private static void BallCollision(DataApi ball1, DataApi ball2)
    {
        const int mass = 200;
        var distanceVector = ball2.Position - ball1.Position;
        float minDistance = ball1.Radius + ball2.Radius;

        if (!(distanceVector.LengthSquared() < minDistance * minDistance)) return;
        var collisionNormal = Vector2.Normalize(distanceVector);

        var relativeVelocity = ball2.Velocity - ball1.Velocity;

        var impulseMagnitude = Vector2.Dot(relativeVelocity, collisionNormal);

        if (impulseMagnitude > 0)
            return;

        var newVelocity1 = (mass - mass) / (mass + mass) * ball1.Velocity +
                           2 * mass / (mass + mass) * ball2.Velocity;
        var newVelocity2 = 2 * mass / (mass + mass) * ball1.Velocity +
                           (mass - mass) / (mass + mass) * ball2.Velocity;

        ball1.Velocity = newVelocity1;
        ball2.Velocity = newVelocity2;
    }

    private void NotifyObservers(LogicApi table)
    {
        foreach (var observer in _observers) observer.OnNext(table);
    }
}

internal class SubscriptionToken(ICollection<IObserver<LogicApi>> observers, IObserver<LogicApi> observer)
    : IDisposable
{
    public void Dispose()
    {
        observers.Remove(observer);
    }
}