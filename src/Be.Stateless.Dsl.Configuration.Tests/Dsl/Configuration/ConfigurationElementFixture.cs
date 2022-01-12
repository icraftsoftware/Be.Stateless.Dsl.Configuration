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
using System.Linq;
using System.Xml.Linq;
using Be.Stateless.Extensions;
using Be.Stateless.Xml.Linq.Extensions;
using Be.Stateless.Xunit;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Be.Stateless.Dsl.Configuration
{
	public class ConfigurationElementFixture
	{
		[Theory]
		[MemberData(nameof(SpecificationApplicationCases))]
		public void Apply(TestCaseData @case)
		{
			//if (@case.Name != "update operation.5") return; // easier debugging

			if (@case.Error.IsNullOrEmpty())
			{
				var result = @case.InputConfiguration.Apply(@case.ForwardSpecification);
				result.Configuration.IsDirty.Should().Be(@case.IsDirty);
				((XDocument) result.Configuration).Should().BeEquivalentTo((XDocument) @case.OutputConfiguration);
				((XDocument) result.Configuration).Root!.Attributes().Where(a => a.IsAnnotation())
					.Should().BeEmpty("resulting configuration root should not have any specification annotation attributes");

				((XDocument) result.InverseSpecification).Should().BeEquivalentTo((XDocument) @case.BackwardSpecification);
				((XDocument) result.Configuration.Apply(result.InverseSpecification).Configuration)
					.Should().BeEquivalentTo((XDocument) @case.InputConfiguration);
			}
			else
			{
				Invoking(() => @case.InputConfiguration.Apply(@case.ForwardSpecification))
					.Should().Throw<InvalidOperationException>()
					.WithMessage(@case.Error);
			}
		}

		public static IEnumerable<object[]> SpecificationApplicationCases
		{
			get
			{
				#region Basic Assumptions

				yield return new TestCaseData("basic assumption.1") {
					InputConfiguration = "<configuration />",
					ForwardSpecification = "<configuration />",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("basic assumption.2") {
					InputConfiguration = "<configuration />",
					ForwardSpecification = "<configuration></configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("basic assumption.3") {
					InputConfiguration = "<configuration></configuration>",
					ForwardSpecification = "<configuration />",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("basic assumption.4") {
					InputConfiguration = "<configuration />",
					ForwardSpecification = "<?xml version='1.0' encoding='utf-8'?><configuration />",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("basic assumption.5") {
					InputConfiguration = "<?xml version='1.0' encoding='utf-8'?><configuration />",
					ForwardSpecification = "<configuration />",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}' />"
				};
				yield return new TestCaseData("basic assumption.8") {
					InputConfiguration = "<configuration />",
					ForwardSpecification = "<root />",
					Error = "Cannot find a configuration element to update that corresponds to specification '/root'."
				};

				#endregion

				#region Comments

				yield return new TestCaseData("comment.1") {
					InputConfiguration = @"<configuration>
	<configSections>
		<!-- the following element needs to be updated -->
		<sectionGroup name='be.stateless' type='stateless.type.one' />
		<sectionGroup name='be.stateless' type='stateless.type.six' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<!-- 
			basically need to update element
			but cannot specify key attributes that would both be unambiguous and allow them to be updated at the same time
			therefore 1st delete and 2nd insert
			-->
		<sectionGroup name='be.stateless' type='stateless.type.one' config:key='name type' config:operation='delete' />
		<sectionGroup name='be.stateless' type='stateless.type.ten' config:operation='insert' />
		<sectionGroup name='be.stateless' type='stateless.type.six' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration>
	<configSections>
		<!-- comments are disregarded by unit tests but the ones from the original configuration will flow through due to initial cloning -->
		<sectionGroup name='be.stateless' type='stateless.type.ten' />
		<sectionGroup name='be.stateless' type='stateless.type.six' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup name='be.stateless' type='stateless.type.one' config:key='name type' config:operation='insert' />
		<sectionGroup name='be.stateless' type='stateless.type.ten' config:operation='delete' />
		<sectionGroup name='be.stateless' type='stateless.type.six' config:operation='none' />
	</configSections>
</configuration>"
				};

				#endregion

				#region Delete Operation

				yield return new TestCaseData("delete operation.1") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.one' />
		<sectionGroup name='be.stateless' type='stateless.type.six' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.one' config:key='name type' config:operation='delete' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.six' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup name='be.stateless' type='stateless.type.one' config:key='name type' config:operation='insert' />
		<sectionGroup name='be.stateless' type='stateless.type.six' config:operation='none' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("delete operation.2") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.one' />
		<sectionGroup name='be.stateless' type='stateless.type.six' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.one' config:key='name type' config:operation='delete' />
		<sectionGroup name='be.stateless.ten' type='stateless.type.ten' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.six' />
		<sectionGroup name='be.stateless.ten' type='stateless.type.ten' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup name='be.stateless' type='stateless.type.one' config:key='name type' config:operation='insert' />
		<sectionGroup name='be.stateless' type='stateless.type.six' config:operation='none' />
		<sectionGroup name='be.stateless.ten' type='stateless.type.ten' config:operation='delete' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("delete operation.3") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.one' />
		<sectionGroup name='be.stateless' type='stateless.type.six' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.one' config:key='name type' config:operation='delete' />
		<sectionGroup name='be.stateless.ten' type='stateless.type.ten' />
		<sectionGroup name='be.stateless' type='stateless.type.six' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless.ten' type='stateless.type.ten' />
		<sectionGroup name='be.stateless' type='stateless.type.six' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup name='be.stateless' type='stateless.type.one' config:key='name type' config:operation='insert' />
		<sectionGroup name='be.stateless.ten' type='stateless.type.ten' config:operation='delete' />
		<sectionGroup name='be.stateless' type='stateless.type.six' config:operation='none' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("delete operation.4") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.one' />
		<sectionGroup name='be.stateless' type='stateless.type.six' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='delete' />
</configuration>",
					OutputConfiguration = @"<configuration />",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='insert'>
		<sectionGroup name='be.stateless' type='stateless.type.one' />
		<sectionGroup name='be.stateless' type='stateless.type.six' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("delete operation.5") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.six' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.one' config:key='name type' config:operation='delete' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup name='be.stateless' type='stateless.type.one' config:key='name type' config:operation='none' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("delete operation.6") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='system.net' type='system.type' />
		<sectionGroup name='be.stateless' type='stateless.type.one' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.two' config:key='name' config:operation='delete' />
	</configSections>
</configuration>",
					Error = "Cannot delete configuration element '/configuration/configSections/sectionGroup[2]' corresponding to "
						+ "specification '/configuration/configSections/sectionGroup' because it conflicts with specification."
				};

				#endregion

				#region Insert Operation

				yield return new TestCaseData("insert operation.0") {
					InputConfiguration = @"<configuration />",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='insert'>
		<sectionGroup name='be.stateless' type='stateless.type' config:operation='insert' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='delete'>
		<sectionGroup name='be.stateless' type='stateless.type' config:operation='delete' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("insert operation.1") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='system.net' type='system.type' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type' config:operation='insert' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='system.net' type='system.type' />
		<sectionGroup name='be.stateless' type='stateless.type' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup name='be.stateless' type='stateless.type' config:operation='delete' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("insert operation.2") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type' config:operation='insert' config:key='name' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup name='be.stateless' type='stateless.type' config:operation='none' config:key='name' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("insert operation.3") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='system.net' type='system.type' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type' config:operation='insert' />
		<sectionGroup name='system.net' config:operation='none' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type' />
		<sectionGroup name='system.net' type='system.type' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup name='be.stateless' type='stateless.type' config:operation='delete' />
		<sectionGroup name='system.net' config:operation='none' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("insert operation.4") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='system.net' type='system.type' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections a='v' config:operation='insert'>
		<sectionGroup name='be.stateless' type='stateless.type' config:operation='insert' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='system.net' type='system.type' />
	</configSections>
	<configSections a='v'>
		<sectionGroup name='be.stateless' type='stateless.type' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections a='v' config:operation='delete'>
		<sectionGroup name='be.stateless' type='stateless.type' config:operation='delete' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("insert operation.5") {
					InputConfiguration = @"<configuration xmlns:s0='urn:0' xmlns:s1='urn:1'>
	<configSections>
		<sectionGroup s0:name='system.net' s1:type='system.type.one' />
		<sectionGroup s0:name='system.net' s1:type='system.type.two' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:s0='urn:0' xmlns:s1='urn:1' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup s0:name='be.stateless' s1:type='stateless.type' config:operation='insert' />
		<sectionGroup s0:name='system.net' s1:type='system.type.one' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration xmlns:s0='urn:0' xmlns:s1='urn:1'>
	<configSections>
		<sectionGroup s0:name='be.stateless' s1:type='stateless.type' />
		<sectionGroup s0:name='system.net' s1:type='system.type.one' />
		<sectionGroup s0:name='system.net' s1:type='system.type.two' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:s0='urn:0' xmlns:s1='urn:1' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup s0:name='be.stateless' s1:type='stateless.type' config:operation='delete' />
		<sectionGroup s0:name='system.net' s1:type='system.type.one' config:operation='none' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("insert operation.6") {
					InputConfiguration = @"<configuration />",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type' config:operation='insert' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='delete'>
		<sectionGroup name='be.stateless' type='stateless.type' config:operation='delete' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("insert operation.7") {
					InputConfiguration = @"<configuration xmlns:s0='urn:0' xmlns:s1='urn:1'>
	<configSections>
		<sectionGroup s0:name='system.net' s1:type='system.type.one' />
		<sectionGroup s0:name='system.net' s1:type='system.type.two' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:s0='urn:0' xmlns:s1='urn:1' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup s0:name='be.stateless' s1:type='stateless.type' config:operation='insert' />
		<sectionGroup s0:name='system.net' />
	</configSections>
</configuration>",
					Error = "Configuration element '/configuration/configSections' contains more than one child configuration elements "
						+ "matching specification '/configuration/configSections/sectionGroup[2]'."
				};

				yield return new TestCaseData("insert operation.8") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='system.net' type='system.type' />
		<sectionGroup name='be.stateless' type='stateless.type.one' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.ten' config:operation='insert' config:key='name' />
	</configSections>
</configuration>",
					Error = "Cannot insert configuration element '/configuration/configSections/sectionGroup[2]' corresponding to "
						+ "specification '/configuration/configSections/sectionGroup' because it conflicts with an existing one."
				};

				#endregion

				#region None Operation

				yield return new TestCaseData("none operation.1") {
					InputConfiguration = @"<configuration />",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' config:key='name' />
		<sectionGroup name='system.net' config:operation='none' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='delete'>
		<sectionGroup name='be.stateless' config:key='name' config:operation='delete' />
		<sectionGroup name='system.net' config:operation='none' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("none operation.2") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='system.net' type='system.type' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' config:key='name' />
		<sectionGroup name='system.net' config:operation='none' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' />
		<sectionGroup name='system.net' type='system.type' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup name='be.stateless' config:key='name' config:operation='delete' />
		<sectionGroup name='system.net' config:operation='none' />
	</configSections>
</configuration>"
				};

				#endregion

				#region Processing Instructions

				yield return new TestCaseData("processing instruction.1") {
					InputConfiguration = @"<configuration>
	<configSections>
		<?processing instruction?>
		<sectionGroup name='be.stateless' type='stateless.type.one' />
		<sectionGroup name='be.stateless' type='stateless.type.six' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.one' config:key='name type' config:operation='delete' />
		<?processing instruction?>
		<sectionGroup name='be.stateless' type='stateless.type.ten' config:operation='insert' />
		<sectionGroup name='be.stateless' type='stateless.type.six' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration>
	<configSections>
		<!-- processing instructions are disregarded by unit tests but the ones from the original configuration will flow through due to initial cloning -->
		<sectionGroup name='be.stateless' type='stateless.type.ten' />
		<sectionGroup name='be.stateless' type='stateless.type.six' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup name='be.stateless' type='stateless.type.one' config:key='name type' config:operation='insert' />
		<sectionGroup name='be.stateless' type='stateless.type.ten' config:operation='delete' />
		<sectionGroup name='be.stateless' type='stateless.type.six' config:operation='none' />
	</configSections>
</configuration>"
				};

				#endregion

				#region Update Operation

				yield return new TestCaseData("update operation.0") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.one' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration a='v' config:key='' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.ten' config:operation='update' config:key='name' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration a='v'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.ten' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration a='v' config:key='' config:scrap='a' config:operation='update' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup name='be.stateless' type='stateless.type.one' config:operation='update' config:key='name' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("update operation.1") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.one' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.ten' config:operation='update' config:key='name' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.ten' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup name='be.stateless' type='stateless.type.one' config:operation='update' config:key='name' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("update operation.2") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.one' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.one' config:operation='update' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup name='be.stateless' type='stateless.type.one' config:operation='none' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("update operation.3") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='system.net' type='system.type' />
		<sectionGroup name='be.stateless' type='stateless.type' a='old' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type' a='new' config:operation='update' config:key='name type' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='system.net' type='system.type' />
		<sectionGroup name='be.stateless' type='stateless.type' a='new' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup name='be.stateless' type='stateless.type' a='old' config:operation='update' config:key='name type' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("update operation.4") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='system.net' type='system.type' />
		<sectionGroup name='be.stateless' type='stateless.type' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type' a='new' config:operation='update' config:key='name type' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='system.net' type='system.type' />
		<sectionGroup name='be.stateless' type='stateless.type' a='new' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup name='be.stateless' type='stateless.type' a='new' config:operation='update' config:scrap='a' config:key='name type' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("update operation.5") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.new' a='new' config:operation='update' config:key='name' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.new' a='new' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup name='be.stateless' type='stateless.type' a='new' config:operation='update' config:scrap='a' config:key='name' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("update operation.6") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='system.net' type='system.type' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type' config:operation='update' />
	</configSections>
</configuration>",
					Error = "Cannot find a configuration element to update that corresponds to specification '/configuration/configSections/sectionGroup'."
				};

				yield return new TestCaseData("update operation.7") {
					InputConfiguration = @"<configuration b='v'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.one' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration a='v' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.ten' config:operation='update' config:key='name' />
	</configSections>
</configuration>",
					Error = "Cannot find a configuration element to update that corresponds to specification '/configuration'."
				};

				yield return new TestCaseData("update operation.8") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.one' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.ten' config:operation='update' config:key='name' config:scrap='type' />
	</configSections>
</configuration>",
					Error = "Cannot scrap attributes of configuration element '/configuration/configSections/sectionGroup' corresponding to "
						+ "specification '/configuration/configSections/sectionGroup' because they conflict with specification ones."
				};

				#endregion

				#region Upsert Operation

				yield return new TestCaseData("upsert operation.1") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='system.net' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' config:key='name' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='system.net' />
		<sectionGroup name='be.stateless' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup name='be.stateless' config:key='name' config:operation='delete' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("upsert operation.2") {
					InputConfiguration = @"<configuration xmlns:s0='urn:0'>
	<configSections>
		<sectionGroup name='be.stateless' s0:value='stateless.type.one' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' xmlns:s0='urn:0'>
	<configSections>
		<sectionGroup name='be.stateless' s0:value='stateless.type.ten' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration xmlns:s0='urn:0'>
	<configSections>
		<sectionGroup name='be.stateless' s0:value='stateless.type.one' />
		<sectionGroup name='be.stateless' s0:value='stateless.type.ten' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}' xmlns:s0='urn:0'>
	<configSections config:operation='none'>
		<sectionGroup name='be.stateless' s0:value='stateless.type.ten' config:operation='delete' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("upsert operation.3") {
					InputConfiguration = @"<configuration xmlns:s0='urn:0'>
	<configSections>
		<sectionGroup name='be.stateless' s0:value='stateless.type.one' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' xmlns:s0='urn:0'>
	<configSections>
		<sectionGroup name='be.stateless' s0:value='stateless.type.ten' config:key='name' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration xmlns:s0='urn:0'>
	<configSections>
		<sectionGroup name='be.stateless' s0:value='stateless.type.ten' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}' xmlns:s0='urn:0'>
	<configSections config:operation='none'>
		<sectionGroup name='be.stateless' s0:value='stateless.type.one' config:key='name' config:operation='update' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("upsert operation.4") {
					InputConfiguration = @"<configuration xmlns:s0='urn:0'>
	<configSections>
		<sectionGroup s0:name='be.stateless' s0:value='stateless.type.one' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup s0:name='be.stateless' s0:value='stateless.type.ten' config:key='{{urn:0}}name' xmlns:s0='urn:0' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration xmlns:s0='urn:0'>
	<configSections>
		<sectionGroup s0:name='be.stateless' s0:value='stateless.type.ten' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup s0:name='be.stateless' s0:value='stateless.type.one' config:key='{{urn:0}}name' config:operation='update' xmlns:s0='urn:0' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("upsert operation.5") {
					InputConfiguration = @"<configuration xmlns:s0='urn:0'>
	<configSections>
		<sectionGroup s0:name='be.stateless' s0:value='stateless.type.one' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' xmlns:s0='urn:0'>
	<configSections>
		<sectionGroup s0:name='be.stateless' s0:value='stateless.type.ten' config:key='{{urn:0}}name' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration xmlns:s0='urn:0'>
	<configSections>
		<sectionGroup s0:name='be.stateless' s0:value='stateless.type.ten' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}' xmlns:s0='urn:0'>
	<configSections config:operation='none'>
		<sectionGroup s0:name='be.stateless' s0:value='stateless.type.one' config:key='{{urn:0}}name' config:operation='update' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("upsert operation.6") {
					InputConfiguration = @"<configuration xmlns:s0='urn:0'>
	<configSections>
		<sectionGroup s0:name='be.stateless' s0:value='stateless.type.one' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}' xmlns:s0='urn:0'>
	<configSections>
		<sectionGroup s0:name='be.stateless' s0:value='stateless.type.ten' config:key='s0:name' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration xmlns:s0='urn:0'>
	<configSections>
		<sectionGroup s0:name='be.stateless' s0:value='stateless.type.ten' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}' xmlns:s0='urn:0'>
	<configSections config:operation='none'>
		<sectionGroup s0:name='be.stateless' s0:value='stateless.type.one' config:key='s0:name' config:operation='update' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("upsert operation.7") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='system.net' type='system.type' />
		<sectionGroup name='be.stateless' type='stateless.type' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup name='be.stateless' type='stateless.type' config:operation='none' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("upsert operation.8") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.one' />
		<sectionGroup name='be.stateless' type='stateless.type.six' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.ten' config:key='name' />
	</configSections>
</configuration>",
					Error = "Configuration element '/configuration/configSections' contains more than one child configuration elements "
						+ "matching specification '/configuration/configSections/sectionGroup'."
				};

				#endregion

				#region Without Operation

				yield return new TestCaseData("without operation.1") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='system.net' type='system.type' />
	</configSections>
</configuration>",
					ForwardSpecification = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type' />
		<sectionGroup name='system.net' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type' />
		<sectionGroup name='system.net' type='system.type' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup name='be.stateless' type='stateless.type' config:operation='delete' />
		<sectionGroup name='system.net' config:operation='none'/>
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("without operation.2") {
					InputConfiguration = @"<configuration xmlns:s0='urn:0' xmlns:s1='urn:1'>
	<configSections>
		<sectionGroup s0:name='system.net' s1:type='system.type' />
	</configSections>
</configuration>",
					ForwardSpecification = @"<configuration xmlns:s0='urn:0' xmlns:s1='urn:1'>
	<configSections>
		<sectionGroup s0:name='be.stateless' s1:type='stateless.type' />
		<sectionGroup s0:name='system.net' s1:type='system.type' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration xmlns:s0='urn:0' xmlns:s1='urn:1'>
	<configSections>
		<sectionGroup s0:name='be.stateless' s1:type='stateless.type' />
		<sectionGroup s0:name='system.net' s1:type='system.type' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:s0='urn:0' xmlns:s1='urn:1' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup s0:name='be.stateless' s1:type='stateless.type' config:operation='delete' />
		<sectionGroup s0:name='system.net' s1:type='system.type' config:operation='none' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("without operation.3") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.one' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.ten' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.one' />
		<sectionGroup name='be.stateless' type='stateless.type.ten' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup name='be.stateless' type='stateless.type.ten' config:operation='delete' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("without operation.4") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.one' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.ten' config:key='name' />
	</configSections>
</configuration>",
					OutputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.ten' />
	</configSections>
</configuration>",
					BackwardSpecification = $@"<configuration config:operation='none' xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections config:operation='none'>
		<sectionGroup name='be.stateless' type='stateless.type.one' config:key='name' config:operation='update' />
	</configSections>
</configuration>"
				};

				yield return new TestCaseData("without operation.5") {
					InputConfiguration = @"<configuration>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.one' />
		<sectionGroup name='be.stateless' type='stateless.type.two' />
	</configSections>
</configuration>",
					ForwardSpecification = $@"<configuration xmlns:config='{Specification.Annotations.NAMESPACE}'>
	<configSections>
		<sectionGroup name='be.stateless' type='stateless.type.ten' config:key='name' />
	</configSections>
</configuration>",
					Error = "Configuration element '/configuration/configSections' contains more than one child configuration elements "
						+ "matching specification '/configuration/configSections/sectionGroup'."
				};

				#endregion
			}
		}
	}
}
