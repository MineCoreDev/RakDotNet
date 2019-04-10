using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;

namespace RakDotNet.Tests
{
    [TestFixture]
    public class ExprTests
    {
        [Test]
        public void Test1() //回数に弱い
        {
            for (int i = 0; i < 100000; i++)
            {
                List<int> list = Activator.CreateInstance<List<int>>();
            }
        }

        [Test]
        public void Test2() //早い
        {
            for (int i = 0; i < 100000; i++)
            {
                List<int> list = new List<int>();
            }
        }

        [Test]
        public void Test3() // 数に強い
        {
            ConstructorInfo info = typeof(List<int>).GetConstructor(new Type[0]);
            Func<List<int>> list = Expression.Lambda<Func<List<int>>>(Expression.New(info)).Compile();
            for (int i = 0; i < 100000; i++)
            {
                list.Invoke();
            }
        }
    }
}