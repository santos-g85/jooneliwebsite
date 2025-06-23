using webjooneli.Models.Entities;

namespace webjooneli.Models.ViewModels
{
    public class CareerViewModel
    {
        public List<JobOpeningModel> JobOpenings { get; set; }

        public CVUploadModel CVUploadModel { get; set; }
    }
}
