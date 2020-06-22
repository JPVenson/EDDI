﻿using Cottle;
using EddiSpeechResponder;
using EddiSpeechService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UnitTests
{
    [TestClass]
    public class ScriptResolverTest : TestBase
    {
        DocumentConfiguration setting;

        [TestInitialize]
        public void start()
        {
            MakeSafe();
            setting = new DocumentConfiguration
            {
                Trimmer = DocumentConfiguration.TrimRepeatedWhitespaces
            };
        }

        [TestMethod]
        public void TestTemplateSimple()
        {
            var document = Document.CreateDefault(@"Hello {name}!", setting).DocumentOrThrow;
            var vars = new Dictionary<Value, Value>();
            vars["name"] = "world";
            var result = document.Render(Context.CreateBuiltin(vars));
            Assert.AreEqual("Hello world!", result);
        }

        [TestMethod]
        public void TestTemplateFunctional()
        {
            var document = Document.CreateDefault(@"You are entering the {P(system)} system.", setting).DocumentOrThrow;
            var vars = new Dictionary<Value, Value>();
            vars["P"] = Value.FromFunction(Function.Create((state, values, output) =>
            {
                return Translations.GetTranslation(values[0].AsString);
            }, 1));
            vars["system"] = "Alrai";
            var result = document.Render(Context.CreateBuiltin(vars));
            Assert.AreEqual("You are entering the <phoneme alphabet=\"ipa\" ph=\"ˈalraɪ\">Alrai</phoneme> system.", result);
        }

        [TestMethod]
        public void TestTemplateConditional()
        {
            var document = Document.CreateDefault("{if value = 1:foo|else:{if value = 2:bar|else:baz}}", setting).DocumentOrThrow;
            var vars = new Dictionary<Value, Value>();
            vars["value"] = 1;
            var result = document.Render(Context.CreateBuiltin(vars));
            Assert.AreEqual("foo", result);
            vars["value"] = 2;
            result = document.Render(Context.CreateBuiltin(vars));
            Assert.AreEqual("bar", result);
            vars["value"] = 3;
            result = document.Render(Context.CreateBuiltin(vars));
            Assert.AreEqual("baz", result);
        }

        [TestMethod]
        public void TestTemplateOneOf()
        {
            Random random = new Random();
            var document = Document.CreateDefault("The letter is {OneOf(\"a\", \"b\", \"c\", \"d\", null)}.", setting).DocumentOrThrow;
            var vars = new Dictionary<Value, Value>();
            vars["OneOf"] = Value.FromFunction(Function.Create((state, values, output) =>
            {
                return values[random.Next(values.Count)];
            }));
            vars["system"] = "Alrai";
            List<string> results = new List<string>();
            for (int i = 0; i < 1000; i++)
            {
                results.Add(document.Render(Context.CreateBuiltin(vars)));
            }
            Assert.IsTrue(results.Contains(@"The letter is a."));
            results.RemoveAll(result => result == @"The letter is a.");
            Assert.IsTrue(results.Contains(@"The letter is b."));
            results.RemoveAll(result => result == @"The letter is b.");
            Assert.IsTrue(results.Contains(@"The letter is c."));
            results.RemoveAll(result => result == @"The letter is c.");
            Assert.IsTrue(results.Contains(@"The letter is d."));
            results.RemoveAll(result => result == @"The letter is d.");
            Assert.IsTrue(results.Contains(@"The letter is ."));
            results.RemoveAll(result => result == @"The letter is .");
            Assert.IsTrue(results.Count == 0);
        }

        [TestMethod]
        public void TestResolverSimple()
        {
            Dictionary<string, Script> scripts = new Dictionary<string, Script>();
            scripts.Add("test", new Script("test", null, false, "Hello {name}"));
            ScriptResolver resolver = new ScriptResolver(scripts);
            Dictionary<Value, Value> dict = new Dictionary<Value, Value>();
            dict["name"] = "world";
            string result = resolver.resolveFromName("test", dict);
            Assert.AreEqual("Hello world", result);
            string result2 = resolver.resolveFromValue(scripts["test"].Value, dict);
            Assert.AreEqual("Hello world", result2);
        }

        [TestMethod]
        public void TestResolverFunctions()
        {
            Dictionary<string, Script> scripts = new Dictionary<string, Script>();
            scripts.Add("func", new Script("func", null, false, "Hello {name}"));
            scripts.Add("test", new Script("test", null, false, "Well {F(\"func\")}"));
            ScriptResolver resolver = new ScriptResolver(scripts);
            Dictionary<Value, Value> dict = new Dictionary<Value, Value>();
            dict["name"] = "world";
            string result = resolver.resolveFromName("test", dict);
            Assert.AreEqual("Well Hello world", result);
            string result2 = resolver.resolveFromValue(scripts["test"].Value, dict);
            Assert.AreEqual("Well Hello world", result2);
        }

        [TestMethod]
        public void TestResolverCallsign()
        {
            Assert.AreEqual(new Regex("[^a-zA-Z0-9]").Replace("a-b. c", "").ToUpperInvariant().Substring(0, 3), "ABC");
        }

        [TestMethod]
        public void TestUpgradeScript_FromDefault()
        {
            Script script = new Script("testScript", "Test script", false, "Test script", 3, "Test script");
            Script newDefaultScript = new Script("testScript", "Updated Test script Description", true, "Updated Test script", 3, "Updated Test script");

            Assert.IsTrue(script.Default);
            Assert.AreEqual(script.Name, newDefaultScript.Name);

            Assert.AreNotEqual(script.Description, newDefaultScript.Description);
            Assert.AreNotEqual(script.Responder, newDefaultScript.Responder);
            Assert.AreNotEqual(script.Value, newDefaultScript.Value);
            Assert.AreNotEqual(script.defaultValue, newDefaultScript.defaultValue);
            Assert.AreEqual(script.Priority, newDefaultScript.Priority);

            Script upgradedScript = Personality.UpgradeScript(script, newDefaultScript);

            Assert.IsTrue(upgradedScript.Default);

            Assert.AreEqual(newDefaultScript.Description, upgradedScript.Description);
            Assert.AreEqual(newDefaultScript.Responder, upgradedScript.Responder);
            Assert.AreEqual(newDefaultScript.Value, upgradedScript.Value);
            Assert.AreEqual(newDefaultScript.defaultValue, upgradedScript.defaultValue);
            Assert.AreEqual(newDefaultScript.Priority, upgradedScript.Priority);
        }

        [TestMethod]
        public void TestUpgradeScript_FromCustomized()
        {
            Script script = new Script("testScript", "Test script", false, "Test script customized", 4, "Test script");
            Script newDefaultScript = new Script("testScript", "Updated Test script Description", false, "Updated Test script", 3, "Updated Test script");

            Assert.IsFalse(script.Default);
            Assert.AreEqual(script.Name, newDefaultScript.Name);

            Assert.AreNotEqual(script.Description, newDefaultScript.Description);
            Assert.AreEqual(script.Responder, newDefaultScript.Responder);
            Assert.AreNotEqual(script.Value, newDefaultScript.Value);
            Assert.AreNotEqual(script.defaultValue, newDefaultScript.defaultValue);
            Assert.AreNotEqual(script.Priority, newDefaultScript.Priority);

            Script upgradedScript = Personality.UpgradeScript(script, newDefaultScript);

            Assert.IsFalse(upgradedScript.Default);

            Assert.AreNotEqual(newDefaultScript.Description, upgradedScript.Description);
            Assert.AreEqual(newDefaultScript.Responder, upgradedScript.Responder);
            Assert.AreNotEqual(newDefaultScript.Value, upgradedScript.Value);
            Assert.AreEqual(newDefaultScript.defaultValue, upgradedScript.defaultValue);
            Assert.AreNotEqual(newDefaultScript.Priority, upgradedScript.Priority);
        }
    }
}
