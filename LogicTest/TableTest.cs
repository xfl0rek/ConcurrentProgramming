﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Data;
using Logic;

namespace LogicTest
{
    internal class FakeDataAPI : DataAPI
    {
        public override int Radius => throw new NotImplementedException();

        public override int Mass => throw new NotImplementedException();

        public override Vector2 Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override Vector2 Velocity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Move(float velocity)
        {
            throw new NotImplementedException();
        }

        public override IDisposable Subscribe(IObserver<DataAPI> observer)
        {
            throw new NotImplementedException();
        }
    }

    [TestClass]
    public class TableTest
    {
        [TestMethod]
        public void CreateAPITest()
        {
            List<FakeDataAPI> fakeDataAPIs = new List<FakeDataAPI>();
            List<DataAPI> balls = fakeDataAPIs.OfType<DataAPI>().ToList<DataAPI>();
            LogicAPI table = LogicAPI.Instance(1000, 1000, balls);
            Assert.IsNotNull(table);    
        }

        [TestMethod]
        public void TestResetTable()
        {
            List<FakeDataAPI> fakeDataAPIs = new List<FakeDataAPI>();
            FakeDataAPI data1 = new FakeDataAPI();
            FakeDataAPI data2 = new FakeDataAPI();
            FakeDataAPI data3 = new FakeDataAPI();
            fakeDataAPIs.Add(data1);
            fakeDataAPIs.Add(data2);
            fakeDataAPIs.Add(data3);
            List<DataAPI> balls = fakeDataAPIs.OfType<DataAPI>().ToList<DataAPI>();
            LogicAPI table = LogicAPI.Instance(1000, 1000, balls);
            table.ResetTable();
            Assert.AreEqual(0, table.GetBallPositions().Count);
        }
    }
}
