using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICC.Predictor.Admin.Models
{
    public class NotificationModel
    {
        public List<Platforms> NotificationPlatforms { get; set; }

        public string NotificationPlatformId { get; set; }
        public string NotificationText { get; set; }
        public int? NotificationMatch { get; set; }

        public string NotifcationTextJson { get; set; }
    }

    public class Platforms
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }


    public class NotificationWorker
    {
        public NotificationModel GetModel()
        {

            NotificationModel model = new NotificationModel();



            #region " Platforms Dropdown "

            List<Platforms> mPlatforms = new List<Platforms>();
            mPlatforms.Add(new Platforms
            {
                Id = "0",
                Name = "Select Platform"
            });
            mPlatforms.Add(new Platforms
            {
                Id = "1",
                Name = "Android"
            });
            mPlatforms.Add(new Platforms
            {
                Id = "2",
                Name = "IOS"
            });
            mPlatforms.Add(new Platforms
            {
                Id = "3",
                Name = "Both"
            });

            model.NotificationPlatforms = mPlatforms;
            #endregion

            return model;

        }
    }

}
