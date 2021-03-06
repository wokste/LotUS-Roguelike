﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using HackConsole.Algo;

namespace HackLibTests
{
    [TestClass]
    public class StringExtTest
    {
        [TestMethod]
        public void StringExt_Wrap() {
            var sentence = "the quick old abcdefghijklmnopqrstuvwxyz jumps over the lazy dog.\n\n:P";
            var lines = sentence.Wrap(10).ToArray();

            Assert.AreEqual("the quick", lines[0]);
            Assert.AreEqual("old", lines[1]);
            Assert.AreEqual("abcdefghij", lines[2]);
            Assert.AreEqual("klmnopqrst", lines[3]);
            Assert.AreEqual("uvwxyz", lines[4]);
            Assert.AreEqual("jumps over", lines[5]);
            Assert.AreEqual("the lazy", lines[6]);
            Assert.AreEqual("dog.", lines[7]);
            Assert.AreEqual("", lines[8]);
            Assert.AreEqual(":P", lines[9]);
        }

        [TestMethod]
        public void StringExt_Prefix()
        {
            var sentences = new string[] { "hello", "world" };
            var lines = sentences.Prefix("* ").ToArray();

            Assert.AreEqual("* hello", lines[0]);
            Assert.AreEqual("  world", lines[1]);
        }

        [TestMethod]
        public void StringExt_CleanUp()
        {
            Assert.AreEqual("double  space.capital!   other capital?\nthird capital.".CleanUp(), "Double space.Capital! Other capital?\nThird capital.");
        }
    }
}
