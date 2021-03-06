﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Globalization;
using CAF.WebSite.Application.Services.Localization;
using CAF.WebSite.Application.Services.Directory;
using CAF.Infrastructure.Core;
using CAF.WebSite.Application.WebUI;

namespace CAF.WebSite.Mvc.Controllers
{
    public partial class CountryController : PublicControllerBase
	{
		#region Fields

        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;

	    #endregion

		#region Constructors

        public CountryController(ICountryService countryService, 
            IStateProvinceService stateProvinceService, 
            ILocalizationService localizationService, 
            IWorkContext workContext,
            ICacheManager cacheManager)
		{
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
		}

        #endregion

        #region States / provinces
        /// <summary>
        /// 返回省/州信息
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="addEmptyStateIfRequired"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetStatesByCountryId(string countryId, bool addEmptyStateIfRequired)
        {
            //this action method gets called via an ajax request
            if (String.IsNullOrEmpty(countryId))
                throw new ArgumentNullException("countryId");

            string cacheKey = string.Format(ModelCacheEventConsumer.STATEPROVINCES_BY_COUNTRY_MODEL_KEY, countryId, addEmptyStateIfRequired, _workContext.WorkingLanguage.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                var country = _countryService.GetCountryById(Convert.ToInt32(countryId));
                var states = _stateProvinceService.GetStateProvincesByCountryId(country != null ? country.Id : 0).ToList();
                var result = (from s in states
                              select new { id = s.Id, name = s.GetLocalized(x => x.Name) })
                              .ToList();

                if (addEmptyStateIfRequired && result.Count == 0)
                    result.Insert(0, new { id = 0, name = _localizationService.GetResource("Address.OtherNonUS") });
                return result;

            });

            return Json(cacheModel, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
