using System.Diagnostics.CodeAnalysis;
using System.Text;
using PABC.MigrationService.Features.DatabaseInitialization;

namespace PABC.Server.Test.MigrationService.Features.DatabaseInitialization
{
    public class DataSetParserTests
    {
        [Fact]
        public async Task Empty_object_fails_with_required_error()
        {
            var error = await TestThrowsJsonSchemaValidationExceptionWithSingleError("""
            {
            }
            """);
            Assert.Equal("required", error.Key);
        }

        [Fact]
        public async Task Missing_applicationRoles_fails_with_required_error()
        {
            var error = await TestThrowsJsonSchemaValidationExceptionWithSingleError("""
            {
                "functionalRoles": [],
                "domains": [],
                "entityTypes": [],
                "mappings": []
            }
            """);
            Assert.Equal("required", error.Key);
        }

        [Fact]
        public async Task Missing_functionalRoles_fails_with_required_error()
        {
            var error = await TestThrowsJsonSchemaValidationExceptionWithSingleError("""
            {
                "applicationRoles": [],
                "domains": [],
                "entityTypes": [],
                "mappings": []
            }
            """);
            Assert.Equal("required", error.Key);
        }

        [Fact]
        public async Task Missing_domains_fails_with_required_error()
        {
            var error = await TestThrowsJsonSchemaValidationExceptionWithSingleError("""
            {
                "applicationRoles": [],
                "functionalRoles": [],
                "entityTypes": [],
                "mappings": []
            }
            """);
            Assert.Equal("required", error.Key);
        }

        [Fact]
        public async Task Missing_entityTypes_fails_with_required_error()
        {
            var error = await TestThrowsJsonSchemaValidationExceptionWithSingleError("""
            {
                "applicationRoles": [],
                "functionalRoles": [],
                "domains": [],
                "mappings": []
            }
            """);
            Assert.Equal("required", error.Key);
        }

        [Fact]
        public async Task Missing_mappings_fails_with_required_error()
        {
            var error = await TestThrowsJsonSchemaValidationExceptionWithSingleError("""
            {
                "applicationRoles": [],
                "functionalRoles": [],
                "domains": [],
                "entityTypes": []
            }
            """);
            Assert.Equal("required", error.Key);
        }

        [Fact]
        public async Task All_root_level_properties_empty_array_succeeds()
        {
            await Test("""
            {
                "applicationRoles": [],
                "functionalRoles": [],
                "domains": [],
                "entityTypes": [],
                "mappings": []
            } 
            """);
        }

        [Fact]
        public async Task Index_property_too_long_fails_with_maxLength_error()
        {
            var error = await TestThrowsJsonSchemaValidationExceptionWithSingleError($$"""
            {
                "applicationRoles": [],
                "functionalRoles": [
                    {
                      "id": "{{Guid.NewGuid()}}",
                      "name": "{{TextOf257Characters}}"
                    }
                ],
                "domains": [],
                "entityTypes": [],
                "mappings": []
            } 
            """);
            Assert.Equal("maxLength", error.Key);
        }

        [Fact]
        public async Task Mapping_with_all_entity_types_and_domainId_fails_with_const_expected_null_error()
        {
            var error = await TestThrowsJsonSchemaValidationExceptionWithSingleError("""
            {
                "applicationRoles": [],
                "functionalRoles": [],
                "domains": [],
                "entityTypes": [],
                "mappings": [
                    {
                      "id": "a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d",
                      "functionalRoleId": "e5a7c8f9-3b12-4d67-9e8f-1a2b3c4d5e6f",
                      "applicationRoleId": "7839298d-9ac9-4b02-bcfc-18b5e6e00227",
                      "isAllEntityTypes": true,
                      "domainId": "7839298d-9ac9-4b02-bcfc-18b5e6e00227"
                    }
                ]
            } 
            """);
            Assert.Equal("const", error.Key);
            Assert.Equal("Expected \"null\"", error.Value);
        }

        private static async Task<DataSet> Test([StringSyntax("json")] string input)
        {
            var parser = new DatasetParser();
            await using var stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes(input));
            stream.Seek(0, SeekOrigin.Begin);
            return await parser.Parse(stream, CancellationToken.None);
        }

        private static async Task<KeyValuePair<string, string>> TestThrowsJsonSchemaValidationExceptionWithSingleError([StringSyntax("json")] string input)
        {
            var exception = await Assert.ThrowsAsync<JsonSchemaValidationException>(() => Test(input));
            var singleRootLevelError = Assert.Single(exception.Errors);
            return Assert.Single(singleRootLevelError.Errors);
        }

        const string TextOf257Characters = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, s";
    }
}
