using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.SecretsManager;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using SecurityService.Interfaces;

namespace SecurityService.Services
{
    public class CognitoUserManagement : ICognitoUserManagement
    {
        private readonly IAmazonSimpleSystemsManagement _client;

        private readonly string _userPoolId;
        private readonly IAmazonCognitoIdentityProvider _provider;
        public CognitoUserManagement()
        {
            _provider = new AmazonCognitoIdentityProviderClient(RegionEndpoint.USEast1);
            _client = new AmazonSimpleSystemsManagementClient(RegionEndpoint.USEast1);
            _userPoolId = GetParameterValueAsync("cognitoUserPoolId").Result;
        }

        public async Task AdminAddUserToGroupAsync(string username, string groupName)
        {
            try
            {

                AdminAddUserToGroupRequest adminAddUserToGroupRequest = new AdminAddUserToGroupRequest
                {
                    Username = username,
                    UserPoolId = _userPoolId,
                    GroupName = groupName
                };
                AdminAddUserToGroupResponse adminAddUserToGroupResponse = await _provider.AdminAddUserToGroupAsync(adminAddUserToGroupRequest).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task AdminRemoveUserFromGroupAsync(string username, string group)
        {
            try
            {

                AdminRemoveUserFromGroupRequest adminRemoveUserFromGroupRequest = new AdminRemoveUserFromGroupRequest
                {
                    Username = username,
                    UserPoolId = _userPoolId,
                    GroupName = group
                };
                AdminRemoveUserFromGroupResponse adminRemoveUserFromGroup = await _provider.AdminRemoveUserFromGroupAsync(adminRemoveUserFromGroupRequest).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CreateCustomAttribute(List<SchemaAttributeType> _customAttributes)
        {

            try
            {
                AddCustomAttributesRequest addCustomAttributesRequest = new AddCustomAttributesRequest
                {
                    UserPoolId = _userPoolId,
                    CustomAttributes = _customAttributes
                };
                AddCustomAttributesResponse addCustomAttributesResponse = await _provider.AddCustomAttributesAsync(addCustomAttributesRequest)
            .ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> AdminUpdateUserAttributesAsync(string username, List<AttributeType> _userAttributes)
        {
            try
            {

                AdminUpdateUserAttributesRequest adminUpdateUserAttributes = new AdminUpdateUserAttributesRequest
                {
                    Username = username,
                    UserAttributes = _userAttributes,
                    UserPoolId = _userPoolId
                };
                AdminUpdateUserAttributesResponse adminAddUserToGroupResponse = await _provider
                .AdminUpdateUserAttributesAsync(adminUpdateUserAttributes)
                .ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CognitoAddNewGroupAsync(string groupname, string groupdescription)
        {
            try
            {

                CreateGroupRequest createGroupRequest = new CreateGroupRequest
                {
                    Description = groupdescription,
                    GroupName = groupname,
                    UserPoolId = _userPoolId

                };
                CreateGroupResponse createGroupResponse = await _provider
                .CreateGroupAsync(createGroupRequest).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetGroupResponse> CognitoGetGroupsAsync(string groupName)
        {
            try
            {

                GetGroupRequest getGroupRequest = new GetGroupRequest
                {
                    GroupName = groupName,
                    UserPoolId = _userPoolId
                };
                GetGroupResponse getGroupResponse = await _provider
                .GetGroupAsync(getGroupRequest).ConfigureAwait(false);
                return getGroupResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ListGroupsResponse> CognitoGetAllGroupsAsync()
        {
            try
            {
                ListGroupsRequest getGroupListRequest = new ListGroupsRequest
                {
                    UserPoolId = _userPoolId
                };
                ListGroupsResponse getGroupResponse = await _provider
                .ListGroupsAsync(getGroupListRequest).ConfigureAwait(false);
                return getGroupResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CognitoUpdateGroupAsync(string groupname, string groupdescription)
        {
            try
            {
                UpdateGroupRequest updateGroupRequest = new UpdateGroupRequest
                {
                    Description = groupdescription,
                    GroupName = groupname,
                    UserPoolId = _userPoolId

                };
                UpdateGroupResponse updateGroupResponse = await _provider
                .UpdateGroupAsync(updateGroupRequest).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CognitoRemoveGroupAsync(string groupname)
        {
            try
            {
                DeleteGroupRequest deleteGroupRequest = new DeleteGroupRequest
                {
                    GroupName = groupname,
                    UserPoolId = _userPoolId

                };
                DeleteGroupResponse deleteGroupResponse = await _provider
                .DeleteGroupAsync(deleteGroupRequest).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ListUsersResponse> GetAllCognitoUsers()
        {
            try
            {
                ListUsersRequest listUsersRequest = new ListUsersRequest
                {
                    UserPoolId = _userPoolId
                };
                ListUsersResponse listUsersResponse = await _provider
                .ListUsersAsync(listUsersRequest).ConfigureAwait(false);
                return listUsersResponse;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<UserType>> GetUsersListByGroupAndRole(string GroupName, string Role)
        {
            try
            {

                var userswithroles = new List<UserType>();
                ListUsersInGroupRequest listUsersInGroupRequest = new ListUsersInGroupRequest
                {
                    GroupName = GroupName,
                    UserPoolId = _userPoolId
                };
                ListUsersInGroupResponse listUsersInGroupResponse = await _provider
                .ListUsersInGroupAsync(listUsersInGroupRequest).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(Role) && listUsersInGroupResponse.Users.Count > 0)
                {

                    var groupfilter = $@"custom:tenant:{GroupName}".ToLower();
                    userswithroles = listUsersInGroupResponse.Users.Where(a => a.Attributes.Any(b => b.Name.Contains(groupfilter) && b.Value == Role)).ToList();

                    return userswithroles;
                }
                else
                {
                    return listUsersInGroupResponse.Users;
                }


                return userswithroles;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<AdminCreateUserResponse> CreateNewCognitoUser(string userEmail, string temporaryPassword, List<AttributeType> userAttributes)
        {
            try
            {
                AdminCreateUserRequest createUserRequest = new AdminCreateUserRequest
                {
                    UserPoolId = _userPoolId,
                    UserAttributes = userAttributes,
                    Username = userEmail,
                    DesiredDeliveryMediums = new List<String> { "EMAIL" },
                    TemporaryPassword = temporaryPassword

                };
                AdminCreateUserResponse createUserResponse = await _provider
                .AdminCreateUserAsync(createUserRequest).ConfigureAwait(false);
                return createUserResponse;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<AdminDisableUserResponse> CognitoDisableUserAsync(string userName)
        {
            try
            {
                AdminDisableUserRequest adminDisableUserRequest = new AdminDisableUserRequest
                {
                    Username = userName,
                    UserPoolId = _userPoolId

                };
                var disableUserResponse = await _provider
                .AdminDisableUserAsync(adminDisableUserRequest).ConfigureAwait(false);
                return disableUserResponse;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<AdminEnableUserResponse> CognitoEnableUserAsync(string userName)
        {
            try
            {
                AdminEnableUserRequest adminEnableUserRequest = new AdminEnableUserRequest
                {
                    Username = userName,
                    UserPoolId = _userPoolId

                };
                var enableUserResponse = await _provider
                .AdminEnableUserAsync(adminEnableUserRequest).ConfigureAwait(false);
                return enableUserResponse;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task ResetUserPassword(string userEmail)
        {
            try
            {
                AdminResetUserPasswordRequest resetPasswordRequest = new AdminResetUserPasswordRequest
                {
                    Username = userEmail,
                    UserPoolId = _userPoolId
                };
                var resetRepsonse = await _provider.AdminResetUserPasswordAsync(resetPasswordRequest).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<AdminGetUserResponse> GetCognitoUser(string userId)
        {
            try
            {
                AdminGetUserRequest getUserRequest = new AdminGetUserRequest
                {

                    UserPoolId = _userPoolId,
                    Username = userId
                };
                AdminGetUserResponse getUserResponse = await _provider
                .AdminGetUserAsync(getUserRequest).ConfigureAwait(false);
                return getUserResponse;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        
        public async Task<DescribeUserPoolResponse> DescribeUserPool()
        {
            try
            {
                DescribeUserPoolRequest describeUserPoolRequest = new DescribeUserPoolRequest
                {
                    UserPoolId = _userPoolId,
                };
                DescribeUserPoolResponse describeUserPoolResponse = await _provider.DescribeUserPoolAsync(describeUserPoolRequest)
            .ConfigureAwait(false);
                return describeUserPoolResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetParameterValueAsync(string parameter)
        {

            var response = await _client.GetParameterAsync(new GetParameterRequest
            {
                Name = parameter,
                WithDecryption = true
            });

            return response.Parameter.Value;
        }

    }
}

