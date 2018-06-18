using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackConsole.Algo;
using System.Diagnostics;

namespace HackLibTests
{
    [TestClass]
    public class WordWrapTest
    {
        [TestMethod]
        public void WordWrap_Wrap() {
            var sentence = "the quick old abcdefghijklmnopqrstuvwxyz jumps over the lazy dog.\n\n:P";
            var lines = WordWrap.Wrap(sentence, 10).ToArray();

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
        public void WordWrap_Prefix()
        {
            var sentences = new string[] { "hello", "world" };
            var lines = WordWrap.Prefix(sentences, "* ").ToArray();

            Assert.AreEqual("* hello", lines[0]);
            Assert.AreEqual("  world", lines[1]);
        }
    }
}
