using System.Threading.Tasks;

using Atlassian.Stash.Entities;
using Atlassian.Stash.Helpers;
using Atlassian.Stash.Workers;

namespace Atlassian.Stash.Api
{
    public class Groups
    {
        private const string MANY_GROUPS = "rest/api/1.0/admin/groups";
        private const string USERS_IN_GROUP = "rest/api/1.0/admin/groups/more-members?context={0}";
        private const string SINGLE_GROUP = MANY_GROUPS + "?name={0}";

        private readonly HttpCommunicationWorker _httpWorker;

        internal Groups(HttpCommunicationWorker httpWorker)
        {
            this._httpWorker = httpWorker;
        }

        public async Task<ResponseWrapper<Group>> Get(RequestOptions requestOptions = null)
        {
            var requestUrl = UrlBuilder.FormatRestApiUrl(MANY_GROUPS, requestOptions);

            var response = await this._httpWorker.GetAsync<ResponseWrapper<Group>>(requestUrl).ConfigureAwait(false);

            return response;
        }

        public async Task<ResponseWrapper<Group>> Get(string filter, RequestOptions requestOptions = null)
        {
            var requestUrl = UrlBuilder.FormatRestApiUrl(MANY_GROUPS + "?filter={0}", requestOptions, filter);

            var response = await this._httpWorker.GetAsync<ResponseWrapper<Group>>(requestUrl).ConfigureAwait(false);

            return response;
        }

        public async Task<ResponseWrapper<User>> GetUsers(string name, RequestOptions requestOptions = null)
        {
            var requestUrl = UrlBuilder.FormatRestApiUrl(USERS_IN_GROUP, requestOptions, name);

            var response = await this._httpWorker.GetAsync<ResponseWrapper<User>>(requestUrl).ConfigureAwait(false);

            return response;
        }

        public async Task<Group> Create(string name)
        {
            var requestUrl = UrlBuilder.FormatRestApiUrl(SINGLE_GROUP, null, name);

            var response = await this._httpWorker.PostAsync<Group>(requestUrl, null).ConfigureAwait(false);

            return response;
        }

        public async Task Delete(string name)
        {
            var requestUrl = UrlBuilder.FormatRestApiUrl(SINGLE_GROUP, null, name);

            await this._httpWorker.DeleteAsync(requestUrl).ConfigureAwait(false);
            
        }
    }
}
