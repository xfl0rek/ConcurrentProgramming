﻿using System.Diagnostics;

namespace Data
{
    public class Ball : DataAPI, IObservable<Ball>
    {
        private float _x;
        private float _y;
        private float _velociyX;
        private float _velocityY;
        private readonly int _radius;
        private readonly object _moveLock = new object();
        private readonly object _velocityLock = new object();
        private List<IObserver<Ball>> _observers;

        public Ball(float x, float y, int radius)
        {
            this._x = x;
            this._y = y;
            this._radius = radius;
            this._observers = new List<IObserver<Ball>>();
        }

        public override float X
        {
            get => _x;
            set
            {
                if (_x != value)
                {
                    lock (_moveLock)
                    {
                        _x = value;
                    }
                }
            }
        }

        public override float Y
        {
            get => _y;
            set
            {
                if (_y != value)
                {
                    lock (_moveLock)
                    {
                        _y = value;
                    }
                }
            }
        }

        public override float VelocityX
        {
            get => _velociyX;
            set
            {
                if (_velociyX != value)
                {
                    lock (_velocityLock)
                    {
                        _velociyX = value;
                    }
                }
            }
        }

        public override float VelocityY
        {
            get => _velocityY;
            set
            {
                if (_velocityY != value)
                {
                    lock (_velocityLock)
                    {
                        _velocityY = value;
                    }
                }
            }
        }

        public override int Radius
        {
            get => _radius;
        }

        public override async Task Move(float velocity)
        {
            var rand = new Random();
            float moveAngle = rand.Next(0, 360);
            VelocityX = velocity * (float)Math.Cos(moveAngle);
            VelocityY = velocity * (float)Math.Sin(moveAngle);
            float timeOfTravel = 1f / 60f;
            while(true)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                float xChange = VelocityX * timeOfTravel;
                float yChange = VelocityY * timeOfTravel;
                lock (_moveLock)
                {
                    _x += xChange;
                    _y += yChange;
                }
                NotifyObservers(this);
                stopwatch.Stop();
                float timeElapsed = (float)stopwatch.Elapsed.TotalSeconds;
                if (timeElapsed < timeOfTravel)
                {
                    await Task.Delay(TimeSpan.FromSeconds(timeOfTravel - timeElapsed));
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(0));
                }
            }
        }

        public override IDisposable Subscribe(IObserver<Ball> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            return new SubscriptionToken(_observers, observer);
        }

        public void NotifyObservers(Ball ball)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(ball);
            }
        }
    }

    public class SubscriptionToken : IDisposable
    {
        private List<IObserver<Ball>> _observers;
        private IObserver<Ball> _observer;

        public SubscriptionToken(List<IObserver<Ball>> observers, IObserver<Ball> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if (_observer != null && _observers.Contains(_observer))
            {
                _observers.Remove(_observer);
            }
        }
    }
}
