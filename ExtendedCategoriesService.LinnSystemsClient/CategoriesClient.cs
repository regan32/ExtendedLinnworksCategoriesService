// // <copyright file="CategoriesClient.cs" company="Paragon Software Group">
// // EXCEPT WHERE OTHERWISE STATED, THE INFORMATION AND SOURCE CODE CONTAINED
// // HEREIN AND IN RELATED FILES IS THE EXCLUSIVE PROPERTY OF PARAGON SOFTWARE
// // GROUP COMPANY AND MAY NOT BE EXAMINED, DISTRIBUTED, DISCLOSED, OR REPRODUCED
// // IN WHOLE OR IN PART WITHOUT EXPLICIT WRITTEN AUTHORIZATION FROM THE COMPANY.
// //
// // Copyright (c) 1994-2017 Paragon Software Group, All rights reserved.
// //
// // UNLESS OTHERWISE AGREED IN A WRITING SIGNED BY THE PARTIES, THIS SOFTWARE IS
// // PROVIDED "AS-IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// // LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
// // PARTICULAR PURPOSE, ALL OF WHICH ARE HEREBY DISCLAIMED. IN NO EVENT SHALL THE
// // AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// // CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// // SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// // INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// // CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// // ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF NOT ADVISED OF
// // THE POSSIBILITY OF SUCH DAMAGE.
// // </copyright>

namespace ExtendedCategoriesService.LinnSystemsClient
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using ExtendedCategoriesService.LinnSystemsClient.Abstractions;
    using ExtendedCategoriesService.LinnSystemsClient.Abstractions.Dto;

    internal class CategoriesClient : BaseLinnWorksClient, ICategoriesClient
    {
        public CategoriesClient(HttpClient httpClient) 
            : base(httpClient)
        {
        }
        
        public Task<Category> CreateCategoryAsync(
            string name, 
            IReadOnlyDictionary<string, IReadOnlyCollection<string>> headers, 
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync<Category>(
                "api/Inventory/CreateCategory",
                HttpMethod.Post,
                body: "categoryName=" + System.Net.WebUtility.UrlEncode(name),
                headers: headers,
                cancellationToken: cancellationToken);
        }

        public Task DeleteCategoryByIdAsync(
            Guid categoryId, 
            IReadOnlyDictionary<string, IReadOnlyCollection<string>> headers,
            CancellationToken cancellationToken = default)
        {
            return SendRequestAsync(
                "api/Inventory/DeleteCategoryById",
                HttpMethod.Post,
                body: "categoryId=" + categoryId,
                headers: headers,
                cancellationToken: cancellationToken);
        }
    }
}