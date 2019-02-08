using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace UnitTest
{
    /// <summary>
    /// ConverterUnitTests の概要の説明
    /// </summary>
    [TestClass]
    public class ConverterUnitTests
    {
        public ConverterUnitTests()
        {
            //
            // TODO: コンストラクター ロジックをここに追加します
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///現在のテストの実行についての情報および機能を
        ///提供するテスト コンテキストを取得または設定します。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 追加のテスト属性
        //
        // テストを作成する際には、次の追加属性を使用できます:
        //
        // クラス内で最初のテストを実行する前に、ClassInitialize を使用してコードを実行してください
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // クラス内のテストをすべて実行したら、ClassCleanup を使用してコードを実行してください
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 各テストを実行する前に、TestInitialize を使用してコードを実行してください
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 各テストを実行した後に、TestCleanup を使用してコードを実行してください
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TypeStruct()
        {
            // テストデータを用意
            List<Item> list = new List<Item>();
            for (int i = 0; i < 20; i++)
            {
                list.Add(new Item()
                {
                    ID = i,
                    Name = "Name of " + i.ToString(),
                    Value = i,
                    UpdateDateTime = DateTime.Now
                });
            }

            // ListをDataTableへ
            var resultTable = Util.Converter.ToDataTable<List<Item>>(list);

            // DataTableをListへ
            var resultList = Util.Converter.ToList<List<Item>>(resultTable);
            CollectionAssert.AreEqual(list, resultList);
        }

        [TestMethod]
        public void TypePrimitive()
        {
            var list = new List<int>();
            for (int i = 0; i < 20; i++)
            {
                list.Add(i);
            }

            // ListをDataTableへ
            var resultTable = Util.Converter.ToDataTable<List<int>>(list);

            // DataTableをListへ
            var resultList = Util.Converter.ToList<List<int>>(resultTable);
            CollectionAssert.AreEqual(list, resultList);
        }

        //
        // string
        //
        [TestMethod]
        public void TypeString()
        {
            var list = new List<string>();
            for (int i = 0; i < 20; i++)
            {
                list.Add(i.ToString());
            }

            // ListをDataTableへ
            var resultTable = Util.Converter.ToDataTable<List<string>>(list);

            // DataTableをListへ
            var resultList = Util.Converter.ToList<List<string>>(resultTable);
            CollectionAssert.AreEqual(list, resultList);
        }

        [TestMethod]
        public void TypeClass()
        {
            var list = new List<Point>();
            for (int i = 0; i < 20; i++)
            {
                list.Add(new Point() { X = i, Y = i });
            }

            // ListをDataTableへ
            var resultTable = Util.Converter.ToDataTable<List<Point>>(list);

            // DataTableをListへ
            var resultList = Util.Converter.ToList<List<Point>>(resultTable);
            CollectionAssert.AreEqual(list, resultList, new PointComparer());
        }

        private struct Item
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public double Value { get; set; }
            public DateTime UpdateDateTime { get; set; }
        }

        private class Point
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        private class PointComparer : System.Collections.IComparer
        {
            public int Compare(object _a, object _b)
            {
                Debug.Assert(_a is Point);
                Debug.Assert(_b is Point);
                Point a = _a as Point;
                Point b = _b as Point;

                if (a.X == b.X && a.Y == b.Y)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
        }
    }
}
