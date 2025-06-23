using webjooneli.Models.Entities;

namespace webjooneli.Models.ViewModels
{
    public class CareerViewModel
    {
        public List<JobOpeningModel> JobOpenings { get; set; } = new List<JobOpeningModel>();

        public CVUploadModel CVUploadModel { get; set; }
    }
}
