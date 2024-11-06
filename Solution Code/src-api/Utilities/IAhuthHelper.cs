using DTOs;

namespace src_api.Utilities
{
    public interface IAuthHelper
    {
        public string getTnumberLDAP(string ShortName);
        public Task<Response_Auth_Server_Data_DTO[]> GetAccessGroups(string base64Username, string base64Password);
        public bool AccessValidator(string base64Username, string base64Password);
        public DTOs.User AuthorizeUser(string base64username, Response_Auth_Server_Data_DTO[] groups,int serverid);
    }
}
