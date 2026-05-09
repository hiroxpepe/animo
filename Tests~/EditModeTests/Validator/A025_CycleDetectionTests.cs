// Copyright (c) STUDIO MeowToon. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable enable

using System.Collections.Generic;
using NUnit.Framework;
using Animo.Core;
using Animo.Model;
using Animo.Tests.EditMode.Helpers;
using static Animo.Tests.EditMode.Helpers.Fixture;

namespace Animo.Tests.EditMode.ValidatorTests {
    /// <summary>Decision-table tests for Validator rule A025: influences has a cycle (Error since v0.1.2).</summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    [TestFixture]
    public class A025_CycleDetectionTests {
        [Test] public void Case01_NoInfluences_Passes() {
            Root root = MinimalRoot();
            ValidationResult r = Validator.Validate(root: root); AssertResult.IsClean(r);
        }
        [Test] public void Case02_OneWayInfluence_Passes() {
            Root root = MinimalRoot();
            root.personas[0].influences = new List<Influence> { InfluenceOf(source: "a", target: "b", coefficient: 0.5f) };
            ValidationResult r = Validator.Validate(root: root); Assert.That(r.HasRule(rule_id: "A025"), Is.False);
        }
        [Test] public void Case03_DirectCycle_FailsA025() {
            Root root = MinimalRoot();
            root.personas[0].influences = new List<Influence> {
                InfluenceOf(source: "a", target: "b", coefficient: 0.5f),
                InfluenceOf(source: "b", target: "a", coefficient: 0.5f) };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A025");
        }
        [Test] public void Case04_TriangleCycle_FailsA025() {
            Root root = MinimalRoot();
            root.personas[0].influences = new List<Influence> {
                InfluenceOf(source: "a", target: "b", coefficient: 0.5f),
                InfluenceOf(source: "b", target: "c", coefficient: 0.5f),
                InfluenceOf(source: "c", target: "a", coefficient: 0.5f) };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A025");
        }
        [Test] public void Case05_SelfReference_FailsA025() {
            Root root = MinimalRoot();
            root.personas[0].influences = new List<Influence> { InfluenceOf(source: "a", target: "a", coefficient: 0.5f) };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A025");
        }
        [Test] public void Case06_MultipleCyclesAndChain_FailsA025() {
            Root root = MinimalRoot();
            root.personas[0].influences = new List<Influence> {
                InfluenceOf(source: "a", target: "b", coefficient: 0.5f),
                InfluenceOf(source: "b", target: "a", coefficient: 0.5f),
                InfluenceOf(source: "c", target: "d", coefficient: 0.5f),
                InfluenceOf(source: "d", target: "c", coefficient: 0.5f) };
            ValidationResult r = Validator.Validate(root: root); AssertResult.HasError(r, rule_id: "A025");
        }
        [Test] public void Case07_IndependentDAGs_Passes() {
            Root root = MinimalRoot();
            root.personas[0].influences = new List<Influence> {
                InfluenceOf(source: "a", target: "b", coefficient: 0.5f),
                InfluenceOf(source: "c", target: "d", coefficient: 0.5f) };
            ValidationResult r = Validator.Validate(root: root); Assert.That(r.HasRule(rule_id: "A025"), Is.False);
        }
    }
}
