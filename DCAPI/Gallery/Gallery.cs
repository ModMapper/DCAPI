
using DCAPI.REST;
using DCAPI.Sessions;
using System.Threading.Tasks;

namespace DCAPI.Gallery {
    /// <summary>디시인사이드의 갤러리입니다.</summary>
    public record Gallery(RESTClient REST, AppToken Token, string Id) {

    }
}
