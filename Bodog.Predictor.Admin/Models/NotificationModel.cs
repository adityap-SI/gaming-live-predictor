using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bodog.Predictor.Admin.Models
{
    public class NotificationModel
    {
        public List<Platforms> NotificationPlatforms { get; set; }

        public String NotificationPlatformId { get; set; }
        public String NotificationText { get; set; }
        public Int32? NotificationMatch { get; set; }

        public String NotifcationTextJson { get; set; }
    }

    public class Platforms
    {
        public String Id { get; set; }
        public String Name { get; set; }
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
