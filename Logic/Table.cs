﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace Logic
{
    public class Table : LogicAPI
    {
        private readonly int _width;
        private readonly int _height;
        private readonly List<DataAPI> _balls;

        public Table(int width, int height)
        {
            this._width = width;
            this._height = height;
            this._balls = new List<DataAPI>();
        }

        public int Width
        {
            get => _width;
        }

        public int Height
        {
            get => _height;
        }

        public List<DataAPI> Balls
        {
            get => _balls;
        }

        public override void CreateBalls(int number, int radius)
        {
            for (int i = 0; i < number; i++)
            {
                var rand = new Random();
                float x = rand.Next(0 + radius, _width - radius);
                float y = rand.Next(0 + radius, _height - radius);
                DataAPI ball = DataAPI.Instance(x, y, radius);
                _balls.Add(ball);
            }
        }

        public override async Task Start(double velocity)
        {
            var rand = new Random();
            foreach (var ball in _balls)
            {
                while (true)
                {
                    float newX = rand.Next(0 + ball.Radius, _width - ball.Radius);
                    float newY = rand.Next(0 + ball.Radius, _height - ball.Radius);
                    await ball.Move(newX, newY, velocity);
                }
            }
        }

        public override List<List<float>> GetBallPositions()
        {
            List<List<float>> positions = new List<List<float>>();
            for (int i = 0; i < _balls.Count; i++)
            {
                List<float> position = new List<float>();
                position.Add(_balls[i].X);
                position.Add(_balls[i].Y);
            }
            return positions;
        }

        public override void ResetTable()
        {
            _balls.Clear();
        }
    }
}
