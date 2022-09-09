using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;


namespace DevelopmentTests
{
    /// <summary>
    ///This is a test class for DependencyGraphTest and is intended
    ///to contain all DependencyGraphTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DependencyGraphTest
    {

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        [TestCategory("1")]
        public void SimpleEmptyTest()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.AreEqual(0, t.Size);
        }


        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyRemoveTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(1, t.Size);
            t.RemoveDependency("x", "y");
            Assert.AreEqual(0, t.Size);
        }


        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyEnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            IEnumerator<string> e1 = t.GetDependees("y").GetEnumerator();
            Assert.IsTrue(e1.MoveNext());
            Assert.AreEqual("x", e1.Current);
            IEnumerator<string> e2 = t.GetDependents("x").GetEnumerator();
            Assert.IsTrue(e2.MoveNext());
            Assert.AreEqual("y", e2.Current);
            t.RemoveDependency("x", "y");
            Assert.IsFalse(t.GetDependees("y").GetEnumerator().MoveNext());
            Assert.IsFalse(t.GetDependents("x").GetEnumerator().MoveNext());
        }


        /// <summary>
        ///Replace on an empty DG shouldn't fail
        ///</summary>
        [TestMethod()]
        public void SimpleReplaceTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(t.Size, 1);
            t.RemoveDependency("x", "y");
            t.ReplaceDependents("x", new HashSet<string>());
            t.ReplaceDependees("y", new HashSet<string>());
        }



        ///<summary>
        ///It should be possibe to have more than one DG at a time.
        ///</summary>
        [TestMethod()]
        public void StaticTest()
        {
            DependencyGraph t1 = new DependencyGraph();
            DependencyGraph t2 = new DependencyGraph();
            t1.AddDependency("x", "y");
            Assert.AreEqual(1, t1.Size);
            Assert.AreEqual(0, t2.Size);
        }




        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void SizeTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            Assert.AreEqual(4, t.Size);
        }


        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void EnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }




        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void ReplaceThenEnumerate()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "b");
            t.AddDependency("a", "z");
            t.ReplaceDependents("b", new HashSet<string>());
            t.AddDependency("y", "b");
            t.ReplaceDependents("a", new HashSet<string>() { "c" });
            t.AddDependency("w", "d");
            t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
            t.ReplaceDependees("d", new HashSet<string>() { "b" });

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }



        /// <summary>
        ///Using lots of data
        ///</summary>
        [TestMethod()]
        public void StressTest()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 200;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 4; j < SIZE; j += 4)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Add some back
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j += 2)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove some more
            for (int i = 0; i < SIZE; i += 2)
            {
                for (int j = i + 3; j < SIZE; j += 3)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            }
        }


        // ################################ TEST WITH YOON VER ################################

        // ################################ 1. EMPTY DEPENDS WITH SINGLE METHOD ################################

        /// <summary>
        /// Empty graph should contain nothing
        /// </summary>
        [TestMethod()]
        [TestCategory("1")]
        public void EmptyDependsTest()
        {
            DependencyGraph t = new DependencyGraph();
            // empty depents
            Assert.IsFalse(t.HasDependents("a"));
            Assert.IsFalse(t.HasDependees("b"));
            // size 0
            Assert.AreEqual(0, t["a"]);
        }


        /// <summary>
        /// Trying to remove empty depends returns nothing.
        /// </summary>
        [TestMethod()]
        [TestCategory("2")]
        public void RemoveDependsTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.RemoveDependency("a", "a");
        }
        

        /// <summary>
        /// Trying to enumerator which is same as iterator.
        /// </summary>
        [TestMethod()]
        [TestCategory("3")]
        public void EnumeratorDependsTest()
        {
            DependencyGraph t = new DependencyGraph();
            // check size of depends if it's empty
            Assert.AreEqual(0, t.GetDependents("a").Count());
            Assert.AreEqual(0, t.GetDependees("b").Count());
            // check enumerator in the depends
            Assert.IsFalse(t.GetDependents("a").GetEnumerator().MoveNext());
            Assert.IsFalse(t.GetDependees("b").GetEnumerator().MoveNext());
        }


        /// <summary>
        /// Trying to replace string with nothing(empty depends).
        /// It should return false since those are empty.
        /// </summary>
        [TestMethod()]
        [TestCategory("4")]
        public void ReplaceDependsTest1()
        {
            DependencyGraph t = new DependencyGraph();
            t.ReplaceDependents("a", new List<string>());
            t.ReplaceDependees("b", new List<string>());

            Assert.IsFalse(t.HasDependents("a"));
            Assert.IsFalse(t.HasDependees("b"));
        }


        /// <summary>
        /// Trying to replace string with nothing(empty depends).
        /// </summary>
        [TestMethod()]
        [TestCategory("5")]
        public void ReplaceDependsTest2()
        {
            DependencyGraph t = new DependencyGraph();
            List<string> list = new List<string>();
            list.Add("b");
            t.ReplaceDependents("a", list);

            Assert.IsTrue(t.HasDependents("a"));
            Assert.IsTrue(t.HasDependees("b"));
        }


        // ################################ 2. EMPTY DEPENDS WITH MANY METHOD ################################


        /// <summary>
        /// Trying to Add and Remove in the same test.
        /// </summary>
        [TestMethod()]
        [TestCategory("6")]
        public void AddAndRemoveSizeTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            Assert.AreEqual(1, t.Size);
            Assert.AreEqual(1, t["b"]);
            Assert.IsTrue(t.HasDependents("a"));
            Assert.IsTrue(t.HasDependees("b"));

            t.RemoveDependency("a", "b");
            Assert.AreEqual(0, t.Size);
            Assert.AreEqual(0, t["b"]);
            Assert.IsFalse(t.HasDependents("a"));
            Assert.IsFalse(t.HasDependees("b"));
        }


        /// <summary>
        /// Trying to Add and Remove in the same test.
        /// And test enumerator.
        /// </summary>
        [TestMethod()]
        [TestCategory("7")]
        public void AddAndRemoveWithEnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            Assert.IsTrue(t.GetDependents("a").GetEnumerator().MoveNext());
            Assert.IsTrue(t.GetDependees("b").GetEnumerator().MoveNext());

            t.RemoveDependency("a", "b");
            Assert.IsFalse(t.GetDependents("a").GetEnumerator().MoveNext());
            Assert.IsFalse(t.GetDependees("b").GetEnumerator().MoveNext());
        }


        /// <summary>
        /// Trying to a lot of Same Add and Remove in the same test.
        /// </summary>
        [TestMethod()]
        [TestCategory("8")]
        public void ManySameAddAndRemoveTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            Assert.AreEqual(1, t.Size);
            Assert.AreEqual(1, t["b"]);
            Assert.IsTrue(t.HasDependents("a"));
            Assert.IsTrue(t.HasDependees("b"));
            t.AddDependency("a", "b");
            t.AddDependency("a", "b");
            t.AddDependency("a", "b");
            t.AddDependency("a", "b");
            Assert.AreEqual(1, t.Size);
            Assert.AreEqual(1, t["b"]);
            Assert.IsTrue(t.HasDependents("a"));
            Assert.IsTrue(t.HasDependees("b"));

            t.RemoveDependency("a", "b");
            Assert.AreEqual(0, t.Size);
            Assert.AreEqual(0, t["b"]);
            Assert.IsFalse(t.HasDependents("a"));
            Assert.IsFalse(t.HasDependees("b"));
            t.RemoveDependency("a", "b");
            t.RemoveDependency("a", "b");
            t.RemoveDependency("a", "b");
            t.RemoveDependency("a", "b");
            Assert.AreEqual(0, t.Size);
            Assert.AreEqual(0, t["b"]);
            Assert.IsFalse(t.HasDependents("a"));
            Assert.IsFalse(t.HasDependees("b"));
        }

        // ################################ 2. VARIABLE PAIRS DEPENDS WITH MANY METHOD ################################


        /// <summary>
        /// Trying to a lot of Diff Add and Remove in the same test.
        /// </summary>
        [TestMethod()]
        [TestCategory("9")]
        public void RepeatAddMethodTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("b", "c");
            t.AddDependency("a", "d");
            t.AddDependency("a", "b");
            t.AddDependency("c", "d");
            Assert.AreEqual(4, t.Size);
            Assert.AreEqual(1, t["b"]);
            Assert.IsTrue(t.HasDependents("a"));
            Assert.IsTrue(t.HasDependents("b"));
            Assert.IsTrue(t.HasDependents("c"));
            Assert.IsFalse(t.HasDependents("d"));
            Assert.IsFalse(t.HasDependees("a"));
            Assert.IsTrue(t.HasDependees("b"));
            Assert.IsTrue(t.HasDependees("c"));
            Assert.IsTrue(t.HasDependees("d"));
        }


        /// <summary>
        /// Trying to a lot of Diff Add and Remove in the same test.
        /// And test enumerator.
        /// </summary>
        [TestMethod()]
        [TestCategory("10")]
        public void RepeatAddMethodWithEnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("b", "c");
            t.AddDependency("a", "d");
            t.AddDependency("a", "b");
            t.AddDependency("c", "d");

            IEnumerator<string> eDPT = t.GetDependents("a").GetEnumerator();
            Assert.IsTrue(eDPT.MoveNext());
            Assert.AreEqual("b", eDPT.Current);
            Assert.IsTrue(eDPT.MoveNext());
            Assert.AreEqual("d", eDPT.Current);
            Assert.IsFalse(eDPT.MoveNext());

            eDPT = t.GetDependents("b").GetEnumerator();
            Assert.IsTrue(eDPT.MoveNext());
            Assert.AreEqual("c", eDPT.Current);
            Assert.IsFalse(eDPT.MoveNext());

            eDPT = t.GetDependents("c").GetEnumerator();
            Assert.IsTrue(eDPT.MoveNext());
            Assert.AreEqual("d", eDPT.Current);
            Assert.IsFalse(eDPT.MoveNext());

            IEnumerator<string> eDEE = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(eDEE.MoveNext());
            Assert.AreEqual("a", eDEE.Current);
            Assert.IsFalse(eDEE.MoveNext());

            eDEE = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(eDEE.MoveNext());
            Assert.AreEqual("b", eDEE.Current);
            Assert.IsFalse(eDEE.MoveNext());

            eDEE = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(eDEE.MoveNext());
            Assert.AreEqual("a", eDEE.Current);
            Assert.IsTrue(eDEE.MoveNext());
            Assert.AreEqual("c", eDEE.Current);
            Assert.IsFalse(eDEE.MoveNext());
        }


        /// <summary>
        /// Trying to an amount of Diff Add and Remove in the same test.
        /// </summary>
        [TestMethod()]
        [TestCategory("11")]
        public void RepeatAddAndRemoveMethodTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");  //1
            t.AddDependency("b", "c");  //2
            t.AddDependency("a", "d");  //3
            t.AddDependency("a", "b");  //3
            t.AddDependency("c", "d");  //4
            t.RemoveDependency("b", "c");   //3
            t.RemoveDependency("b", "e");   //3
            t.AddDependency("a", "e");  //4
            t.AddDependency("e", "d");  //5
            t.AddDependency("f", "d");  //6
            t.AddDependency("g", "d");  //7
            t.RemoveDependency("g", "d");   //6

            Assert.AreEqual(6, t.Size);
            Assert.AreEqual(4, t["d"]);
        }


        /// <summary>
        /// Trying to an amount of Diff Add and Remove in the same test.
        /// and test enumerator.
        /// </summary>
        [TestMethod()]
        [TestCategory("12")]
        public void RepeatAARWithEnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");  //1
            t.AddDependency("b", "c");  //2
            t.AddDependency("a", "d");  //3
            t.AddDependency("a", "b");  //3
            t.AddDependency("c", "d");  //4
            t.RemoveDependency("b", "c");   //3
            t.RemoveDependency("b", "e");   //3
            t.AddDependency("a", "e");  //4
            t.AddDependency("e", "d");  //5
            t.AddDependency("f", "d");  //6
            t.AddDependency("g", "d");  //7
            t.RemoveDependency("g", "d");   //6

            // Enumerator with Dependents
            IEnumerator<string> eDPT = t.GetDependents("a").GetEnumerator();
            Assert.IsTrue(eDPT.MoveNext());
            Assert.AreEqual("b", eDPT.Current);
            Assert.IsTrue(eDPT.MoveNext());
            Assert.AreEqual("d", eDPT.Current);
            Assert.IsTrue(eDPT.MoveNext());
            Assert.AreEqual("e", eDPT.Current);
            Assert.IsFalse(eDPT.MoveNext());

            eDPT = t.GetDependents("b").GetEnumerator();
            Assert.IsFalse(eDPT.MoveNext());

            eDPT = t.GetDependents("c").GetEnumerator();
            Assert.IsTrue(eDPT.MoveNext());
            Assert.AreEqual("d", eDPT.Current);
            Assert.IsFalse(eDPT.MoveNext());

            eDPT = t.GetDependents("d").GetEnumerator();
            Assert.IsFalse(eDPT.MoveNext());

            eDPT = t.GetDependents("e").GetEnumerator();
            Assert.IsTrue(eDPT.MoveNext());
            Assert.AreEqual("d", eDPT.Current);
            Assert.IsFalse(eDPT.MoveNext());

            eDPT = t.GetDependents("f").GetEnumerator();
            Assert.IsTrue(eDPT.MoveNext());
            Assert.AreEqual("d", eDPT.Current);
            Assert.IsFalse(eDPT.MoveNext());

            // Enumerator with Dependees
            IEnumerator<string> eDEE = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(eDEE.MoveNext());
            Assert.AreEqual("a", eDEE.Current);
            Assert.IsFalse(eDEE.MoveNext());

            eDEE = t.GetDependees("c").GetEnumerator();
            Assert.IsFalse(eDEE.MoveNext());

            eDEE = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(eDEE.MoveNext());
            Assert.AreEqual("a", eDEE.Current);
            Assert.IsTrue(eDEE.MoveNext());
            Assert.AreEqual("c", eDEE.Current);
            Assert.IsTrue(eDEE.MoveNext());
            Assert.AreEqual("e", eDEE.Current);
            Assert.IsTrue(eDEE.MoveNext());
            Assert.AreEqual("f", eDEE.Current);
            Assert.IsFalse(eDEE.MoveNext());

            eDEE = t.GetDependees("e").GetEnumerator();
            Assert.IsTrue(eDEE.MoveNext());
            Assert.AreEqual("a", eDEE.Current);
            Assert.IsFalse(eDEE.MoveNext());

            eDEE = t.GetDependees("f").GetEnumerator();
            Assert.IsFalse(eDEE.MoveNext());
        }


        /// <summary>
        /// Trying to an amount of Diff Add, Remove and Replace Depts in the same test.
        /// </summary>
        [TestMethod()]
        [TestCategory("13")]
        public void RepeatAddAndRemoveWithReplaceMethodTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");  //1
            t.AddDependency("b", "c");  //2
            t.AddDependency("a", "d");  //3
            t.AddDependency("a", "b");  //3
            t.AddDependency("c", "d");  //4
            t.RemoveDependency("b", "c");   //3
            t.RemoveDependency("b", "e");   //3
            t.AddDependency("a", "e");  //4
            t.AddDependency("e", "d");  //5
            t.AddDependency("f", "d");  //6
            t.AddDependency("g", "d");  //7
            t.RemoveDependency("g", "d");   //6

            List<string> list = new List<string>();
            list.Add("z"); list.Add("y"); 
            t.ReplaceDependents("a", list);

            // Enumerator with Dependents
            IEnumerator<string> eDPT = t.GetDependents("a").GetEnumerator();
            Assert.IsTrue(eDPT.MoveNext());
            Assert.AreEqual("z", eDPT.Current);
            Assert.IsTrue(eDPT.MoveNext());
            Assert.AreEqual("y", eDPT.Current);
            Assert.IsFalse(eDPT.MoveNext());

            Assert.IsTrue(t.HasDependees("z"));
            Assert.IsTrue(t.HasDependees("y"));
            Assert.IsFalse(t.HasDependees("b"));
            Assert.IsFalse(t.HasDependees("e"));
        }


        /// <summary>
        /// Trying to an amount of Diff Add, Remove and Replace Dees in the same test.
        /// </summary>
        [TestMethod()]
        [TestCategory("14")]
        public void RepeatAddAndRemoveWithReplaceMethodTest2()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");  //1
            t.AddDependency("b", "c");  //2
            t.AddDependency("a", "d");  //3
            t.AddDependency("a", "b");  //3
            t.AddDependency("c", "d");  //4
            t.RemoveDependency("b", "c");   //3
            t.RemoveDependency("b", "e");   //3
            t.AddDependency("a", "e");  //4
            t.AddDependency("e", "d");  //5
            t.AddDependency("f", "d");  //6
            t.AddDependency("g", "d");  //7
            t.RemoveDependency("g", "d");   //6

            List<string> list = new List<string>();
            list.Add("z"); list.Add("y");
            t.ReplaceDependees("d", list);

            // Enumerator with Dependents
            IEnumerator<string> eDPT = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(eDPT.MoveNext());
            Assert.AreEqual("z", eDPT.Current);
            Assert.IsTrue(eDPT.MoveNext());
            Assert.AreEqual("y", eDPT.Current);
            Assert.IsFalse(eDPT.MoveNext());

            Assert.IsTrue(t.HasDependents("z"));
            Assert.IsTrue(t.HasDependents("y"));
            Assert.IsTrue(t.HasDependents("a"));
            Assert.IsFalse(t.HasDependents("c"));
            Assert.IsFalse(t.HasDependents("e"));
            Assert.IsFalse(t.HasDependents("f"));
        }


        /// <summary>
        /// Trying to an amount of Diff Add, Remove and Replace Both in the same test.
        /// and test enumerator.
        /// </summary>
        [TestMethod()]
        [TestCategory("15")]
        public void RepeatAddAndRemoveWithReplaceMethodTest3()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");  //1
            t.AddDependency("b", "c");  //2
            t.AddDependency("a", "d");  //3
            t.AddDependency("a", "b");  //3
            t.AddDependency("c", "d");  //4
            t.RemoveDependency("b", "c");   //3
            t.RemoveDependency("b", "e");   //3
            t.AddDependency("a", "e");  //4
            t.AddDependency("e", "d");  //5
            t.AddDependency("f", "d");  //6
            t.AddDependency("g", "d");  //7
            t.RemoveDependency("g", "d");   //6

            // Replace depts
            List<string> list = new List<string>();
            list.Add("z"); list.Add("y");
            t.ReplaceDependents("a", list);

            List<string> list2 = new List<string>();
            list2.Add("t"); list2.Add("u"); list2.Add("v");
            t.ReplaceDependents("a", list2);

            Assert.IsTrue(t.HasDependees("t"));
            Assert.IsTrue(t.HasDependees("u"));
            Assert.IsTrue(t.HasDependees("v"));
            Assert.IsFalse(t.HasDependees("z"));
            Assert.IsFalse(t.HasDependees("y"));

            t.ReplaceDependents("b", list);
            Assert.IsTrue(t.HasDependees("z"));
            Assert.IsTrue(t.HasDependees("y"));

            // Enumerator with Dependents
            IEnumerator<string> eDPT = t.GetDependents("a").GetEnumerator();
            Assert.IsTrue(eDPT.MoveNext());
            Assert.AreEqual("t", eDPT.Current);
            Assert.IsTrue(eDPT.MoveNext());
            Assert.AreEqual("u", eDPT.Current);
            Assert.IsTrue(eDPT.MoveNext());
            Assert.AreEqual("v", eDPT.Current);
            Assert.IsFalse(eDPT.MoveNext());

            eDPT = t.GetDependents("b").GetEnumerator();
            Assert.IsTrue(eDPT.MoveNext());
            Assert.AreEqual("z", eDPT.Current);
            Assert.IsTrue(eDPT.MoveNext());
            Assert.AreEqual("y", eDPT.Current);
            Assert.IsFalse(eDPT.MoveNext());

            // Replace dees
            t.ReplaceDependees("d", list);
            t.ReplaceDependees("d", list2);
            Assert.IsTrue(t.HasDependents("t"));
            Assert.IsTrue(t.HasDependents("u"));
            Assert.IsTrue(t.HasDependents("v"));
            Assert.IsFalse(t.HasDependents("z"));
            Assert.IsFalse(t.HasDependents("y"));

            // Enumerator with Dependees
            IEnumerator<string> eDEE = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(eDEE.MoveNext());
            Assert.AreEqual("t", eDEE.Current);
            Assert.IsTrue(eDEE.MoveNext());
            Assert.AreEqual("u", eDEE.Current);
            Assert.IsTrue(eDEE.MoveNext());
            Assert.AreEqual("v", eDEE.Current);
            Assert.IsFalse(eDEE.MoveNext());
        }
    }
}