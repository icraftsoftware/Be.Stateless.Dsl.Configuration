#region Copyright & License

// Copyright © 2012 - 2021 François Chabot & Emmanuel Benitez
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;
using Be.Stateless.Xml.Linq.Extensions;
using Be.Stateless.Xunit;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.Dsl.Configuration
{
	public class SpecificationElementFixture
	{
		[Theory]
		[MemberData(nameof(EquatingConfigurations))]
		public void IsEquatedBy(TestCaseData @case)
		{
			@case.ForwardSpecification.Root.SpecificationElements().First()
				.IsEquatedBy(@case.InputConfiguration.Root.ConfigurationElements().First())
				.Should().BeTrue();
		}

		[Theory]
		[MemberData(nameof(UnequatingConfigurations))]
		public void IsNotEquatedBy(TestCaseData @case)
		{
			@case.ForwardSpecification.Root.SpecificationElements().First()
				.IsEquatedBy(@case.InputConfiguration.Root.ConfigurationElements().First())
				.Should().BeFalse();
		}

		[Theory]
		[MemberData(nameof(UnsatisfyingConfigurations))]
		public void IsNotSatisfiedBy(TestCaseData @case)
		{
			@case.ForwardSpecification.Root.SpecificationElements().First()
				.IsSatisfiedBy(@case.InputConfiguration.Root.ConfigurationElements().First())
				.Should().BeFalse();
		}

		[Theory]
		[MemberData(nameof(SatisfyingConfigurations))]
		public void IsSatisfiedBy(TestCaseData @case)
		{
			@case.ForwardSpecification.Root.SpecificationElements().First()
				.IsSatisfiedBy(@case.InputConfiguration.Root.ConfigurationElements().First())
				.Should().BeTrue();
		}

		[Fact]
		public void IsSatisfiedByThrowsWhenKeyAttributeIsNotFoundAmongSpecificationAttributes()
		{
			Configuration configuration = "<configuration a='v' s0:b='v' xmlns:s0='urn:0' />";
			Specification specification = $"<configuration a='v' config:key='a {{urn:0}}b' xmlns:config='{Specification.Annotations.NAMESPACE}' />";
			Invoking(() => specification.Root.SpecificationElements().First().IsSatisfiedBy(configuration.Root.ConfigurationElements().First()))
				.Should().Throw<InvalidOperationException>()
				.WithMessage(
					"Specification attribute '{urn:0}b' was not found among the attributes of the specification element '/configuration' although it was specified as a key attribute.");
		}

		[Theory]
		[MemberData(nameof(AnnotatedSpecificationElements))]
		public void KeyAttributeNames(TestCaseData @case)
		{
			@case.ForwardSpecification.Root.SpecificationElements().First().KeyAttributeNames
				.Should().BeEquivalentTo(@case.ForwardSpecification.Root.SpecificationElements().First().SpecificationAttributeNames);
		}

		[Fact]
		public void OperationGetter()
		{
			var element = new XElement("element");
			var sut = new SpecificationElement(element);
			sut.Operation.Should().Be(Specification.Annotations.Operation.UPSERT);

			element.SetAttributeValue(Specification.Annotations.Attributes.OPERATION, "none");
			sut.Operation.Should().Be(Specification.Annotations.Operation.NONE);
		}

		[Fact]
		public void OperationSetter()
		{
			var element = new XElement("element");
			var _ = new SpecificationElement(element) {
				Operation = Specification.Annotations.Operation.DELETE
			};
			element.Attribute(Specification.Annotations.Attributes.OPERATION)!.Value.Should().Be(Specification.Annotations.Operation.DELETE);
		}

		[Fact]
		public void ScrapAttributeNamesGetter()
		{
			Specification specification = "<configuration s0:a='v' config:key='s0:a' xmlns:s0='urn:0' "
				+ $"xmlns:config='{Specification.Annotations.NAMESPACE}' />";
			specification.Root.SpecificationElements().First().ScrapAttributeNames.Should().BeEmpty();

			specification = "<configuration s0:a='v' s0:b='v' config:key='{{urn:0}}a s0:b' config:scrap='s0:b' xmlns:s0='urn:0' "
				+ $"xmlns:config='{Specification.Annotations.NAMESPACE}' />";
			specification.Root.SpecificationElements().First().ScrapAttributeNames.Should().BeEquivalentTo(new[] { (XNamespace) "urn:0" + "b" });
		}

		[Fact]
		public void ScrapAttributeNamesSetter()
		{
			var element = new XElement("element");
			var sut = new SpecificationElement(element);
			sut.ScrapAttributeNames.Should().BeEmpty();

			var names = new XName[] { "a", "b", "c" };
			sut.ScrapAttributeNames = names;
			element.Attribute(Specification.Annotations.Attributes.SCRAP)!.Value.Should().Be("a b c");
			sut.ScrapAttributeNames.Should().BeEquivalentTo(names);

			sut = new(element);
			element.SetAttributeValue(Specification.Annotations.Attributes.SCRAP, "x|y|z");
			sut.ScrapAttributeNames.Should().BeEquivalentTo(new XName[] { "x", "y", "z" });

			names = new XName[] { "d", "e", "f" };
			sut.ScrapAttributeNames = names;
			element.Attribute(Specification.Annotations.Attributes.SCRAP)!.Value.Should().Be("d e f");
			sut.ScrapAttributeNames.Should().BeEquivalentTo(names);

			sut.ScrapAttributeNames = Array.Empty<XName>();
			element.Attribute(Specification.Annotations.Attributes.SCRAP).Should().BeNull();
			sut.ScrapAttributeNames.Should().BeEmpty();
		}

		[Theory]
		[MemberData(nameof(AnnotatedSpecificationElements))]
		public void ScrapAttributes(TestCaseData @case)
		{
			var specificationElement = @case.ForwardSpecification.Root.SpecificationElements().First();

			specificationElement.ScrapAttributes().Select(sa => sa.Name)
				.Should().BeEquivalentTo(specificationElement.ScrapAttributeNames);

			specificationElement.ScrapAttributeNames.Select(n => specificationElement.SpecificationAttribute(n))
				.Should().BeEquivalentTo(specificationElement.ScrapAttributes());
		}

		[Theory]
		[MemberData(nameof(AnnotatedSpecificationElements))]
		public void SpecificationAttributeNames(TestCaseData @case)
		{
			@case.ForwardSpecification.Root.SpecificationElements().First().SpecificationAttributeNames
				.Should().BeEquivalentTo(
					((XElement) @case.ForwardSpecification.Root.SpecificationElements().First())
					.Attributes()
					.Where(a => !a.IsNamespaceDeclaration && !a.IsAnnotation())
					.Select(a => a.Name)
				);
		}

		[Theory]
		[MemberData(nameof(AnnotatedSpecificationElements))]
		public void SpecificationAttributes(TestCaseData @case)
		{
			@case.ForwardSpecification.Root.SpecificationElements().First().SpecificationAttributes().Select(sa => (XAttribute) sa)
				.Should().BeEquivalentTo(
					((XElement) @case.ForwardSpecification.Root.SpecificationElements().First())
					.Attributes()
					.Where(a => !a.IsNamespaceDeclaration && !a.IsAnnotation())
				);
		}

		public static IEnumerable<object[]> AnnotatedSpecificationElements
		{
			get
			{
				yield return new TestCaseData("explicit keys.1") {
					ForwardSpecification = $"<configuration s0:a='v' config:key='s0:a' xmlns:s0='urn:0' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("explicit keys.2") {
					ForwardSpecification = $"<configuration s0:a='v' config:key='{{urn:0}}a' xmlns:s0='urn:0' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("explicit keys.3") {
					ForwardSpecification =
						$"<configuration s0:a='v' s0:b='v' config:key='{{urn:0}}a s0:b' config:scrap='s0:b' xmlns:s0='urn:0' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("implicit keys.1") {
					ForwardSpecification = "<configuration />"
				};
				yield return new TestCaseData("implicit keys.2") {
					ForwardSpecification = "<configuration a='v' />"
				};
				yield return new TestCaseData("implicit keys.3") {
					ForwardSpecification = "<configuration s0:a='v' xmlns:s0='urn:0' />"
				};
				yield return new TestCaseData("implicit keys.4") {
					ForwardSpecification = $"<configuration s0:a='v' s0:b='v' xmlns:s0='urn:0' config:scrap='s0:b' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
			}
		}

		public static IEnumerable<object[]> EquatingConfigurations
		{
			get
			{
				yield return new TestCaseData("1") {
					InputConfiguration = "<configuration />",
					ForwardSpecification = "<configuration />"
				};
				yield return new TestCaseData("2") {
					InputConfiguration = "<configuration a='v' />",
					ForwardSpecification = "<configuration a='v' />"
				};
				yield return new TestCaseData("3") {
					InputConfiguration = "<configuration s0:a='v' xmlns:s0='urn:0' />",
					ForwardSpecification = "<configuration s1:a='v' xmlns:s1='urn:0' />"
				};
				yield return new TestCaseData("4") {
					InputConfiguration = "<configuration s0:a='v' xmlns:s0='urn:0' xmlns:s1='urn:1' />",
					ForwardSpecification = "<configuration s1:a='v' xmlns:s1='urn:0' />"
				};
				yield return new TestCaseData("5") {
					InputConfiguration = "<configuration a='v' />",
					ForwardSpecification = $"<configuration a='v' config:key='a' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("6") {
					InputConfiguration = "<configuration a='v' s0:b='v' xmlns:s0='urn:0' />",
					ForwardSpecification = $"<configuration a='v' s1:b='v' config:key='a' xmlns:s1='urn:0' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("7") {
					InputConfiguration = "<configuration s0:a='v' s0:b='v' xmlns:s0='urn:0' />",
					ForwardSpecification = $"<configuration s1:a='v' s1:b='v' config:key='s1:a' xmlns:s1='urn:0' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("8") {
					InputConfiguration = "<configuration s0:a='v' s1:b='v' xmlns:s0='urn:0' xmlns:s1='urn:1' />",
					ForwardSpecification = "<configuration s0:a='v' s1:b='v' config:key='{urn:0}a {urn:1}b ' xmlns:s0='urn:0' xmlns:s1='urn:1' "
						+ $"xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("9") {
					InputConfiguration = "<sectionGroup s0:name='be.stateless' s0:value='same' xmlns:s0='urn:0' />",
					ForwardSpecification = "<sectionGroup s0:name='be.stateless' s0:value='same' config:key='{urn:0}name' "
						+ $"xmlns:s0='urn:0' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
			}
		}

		[SuppressMessage("ReSharper", "IdentifierTypo")]
		public static IEnumerable<object[]> UnequatingConfigurations
		{
			get
			{
				yield return new TestCaseData("1") {
					InputConfiguration = "<configuration a='v' />",
					ForwardSpecification = "<configuration />"
				};
				yield return new TestCaseData("2") {
					InputConfiguration = "<configuration a='v' s0:b='v' xmlns:s0='urn:0' />",
					ForwardSpecification = $"<configuration a='v' config:key='a' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("3") {
					InputConfiguration = "<configuration s0:a='v' s0:b='v' xmlns:s0='urn:0' />",
					ForwardSpecification = $"<configuration s0:a='v' config:key='s0:a' xmlns:s0='urn:0' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("4") {
					InputConfiguration = "<configuration s0:a='v' xmlns:s0='urn:0' />",
					ForwardSpecification = $"<configuration s1:a='v' s1:b='v' config:key='s1:a' xmlns:s1='urn:0' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("5") {
					InputConfiguration = "<configuration s0:a='v' s0:b='v' xmlns:s0='urn:0' />",
					ForwardSpecification = $"<configuration s0:a='v' config:key='{{urn:0}}a' xmlns:s0='urn:0' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("6") {
					InputConfiguration = "<configuration s0:a='v' s0:b='v' xmlns:s0='urn:0' />",
					ForwardSpecification = $"<configuration s1:a='v' config:key='s1:a' xmlns:s1='urn:0' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("7") {
					InputConfiguration = "<configuration s0:a='v' s0:b='v' xmlns:s0='urn:0' />",
					ForwardSpecification = $"<configuration s0:a='v' config:key='{{urn:0}}a' xmlns:s0='urn:0' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("8") {
					InputConfiguration = "<sectionGroup s0:name='be.stateless' s0:value='old' xmlns:s0='urn:0' />",
					ForwardSpecification = "<sectionGroup s0:name='be.stateless' s0:value='new' config:key='{urn:0}name' "
						+ $"xmlns:s0='urn:0' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
			}
		}

		public static IEnumerable<object[]> SatisfyingConfigurations
		{
			get
			{
				yield return new TestCaseData("0") {
					InputConfiguration = "<configuration />",
					ForwardSpecification = "<configuration />"
				};
				yield return new TestCaseData("1") {
					InputConfiguration = "<configuration a='v' />",
					ForwardSpecification = "<configuration />"
				};
				yield return new TestCaseData("2") {
					InputConfiguration = "<configuration a='v' />",
					ForwardSpecification = "<configuration a='v' />"
				};
				yield return new TestCaseData("3") {
					InputConfiguration = "<configuration s0:a='v' xmlns:s0='urn:0' />",
					ForwardSpecification = "<configuration s1:a='v' xmlns:s1='urn:0' />"
				};
				yield return new TestCaseData("4") {
					InputConfiguration = "<configuration s0:a='v' xmlns:s0='urn:0' xmlns:s1='urn:1' />",
					ForwardSpecification = "<configuration s1:a='v' xmlns:s1='urn:0' />"
				};
				yield return new TestCaseData("5") {
					InputConfiguration = "<configuration a='v' />",
					ForwardSpecification = $"<configuration a='v' config:key='a' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("6") {
					InputConfiguration = "<configuration a='v' s0:b='v' xmlns:s0='urn:0' />",
					ForwardSpecification = $"<configuration a='v' config:key='a' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("7") {
					InputConfiguration = "<configuration s0:a='v' s0:b='v' xmlns:s0='urn:0' />",
					ForwardSpecification = $"<configuration s0:a='v' config:key='s0:a' xmlns:s0='urn:0' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("8") {
					InputConfiguration = "<configuration s0:a='v' s0:b='v' xmlns:s0='urn:0' />",
					ForwardSpecification = $"<configuration s1:a='v' config:key='s1:a' xmlns:s1='urn:0' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("9") {
					InputConfiguration = "<configuration s0:a='v' s0:b='v' xmlns:s0='urn:0' />",
					ForwardSpecification = $"<configuration s0:a='v' config:key='{{urn:0}}a' xmlns:s0='urn:0' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("A") {
					InputConfiguration = "<configuration s0:a='v' s1:b='v' xmlns:s0='urn:0' xmlns:s1='urn:1' />",
					ForwardSpecification = "<configuration s0:a='v' s1:b='v' config:key='{urn:0}a {urn:1}b ' xmlns:s0='urn:0' xmlns:s1='urn:1' "
						+ $"xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("B") {
					InputConfiguration = "<sectionGroup s0:name='be.stateless' s0:value='old' xmlns:s0='urn:0' />",
					ForwardSpecification = "<sectionGroup s0:name='be.stateless' s0:value='new' config:key='{urn:0}name' "
						+ $"xmlns:s0='urn:0' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
			}
		}

		public static IEnumerable<object[]> UnsatisfyingConfigurations
		{
			get
			{
				yield return new TestCaseData("1") {
					InputConfiguration = "<this />",
					ForwardSpecification = "<that />"
				};
				yield return new TestCaseData("2") {
					InputConfiguration = "<configuration />",
					ForwardSpecification = "<configuration a='v' />"
				};
				yield return new TestCaseData("3") {
					InputConfiguration = "<configuration a='v2' />",
					ForwardSpecification = "<configuration a='v1' />"
				};
				yield return new TestCaseData("5") {
					InputConfiguration = "<configuration b='v' />",
					ForwardSpecification = $"<configuration a='v' config:key='a' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("6") {
					InputConfiguration = "<configuration s0:b='v' xmlns:s0='urn:0' />",
					ForwardSpecification = $"<configuration s1:a='v' config:key='s1:a' xmlns:s1='urn:1' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
			}
		}
	}
}
