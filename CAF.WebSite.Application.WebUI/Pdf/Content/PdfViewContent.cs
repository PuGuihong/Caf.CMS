﻿using CAF.Infrastructure.Core;
using CAF.WebSite.Application.Services.Pdf;
using CAF.WebSite.Application.WebUI.Controllers;
using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
 

namespace CAF.WebSite.Application.WebUI.Pdf
{
	
	public class PdfViewContent : PdfHtmlContent
	{

		public PdfViewContent(string viewName, object model, ControllerContext controllerContext)
			: this(viewName, null, model, controllerContext)
		{
		}

		public PdfViewContent(string viewName, string masterName, object model, ControllerContext controllerContext)
			: base(ViewToString(viewName, masterName, model, false, controllerContext, true))
		{
		}

		protected internal static string ViewToString(string viewName, string masterName, object model, bool isPartial, ControllerContext context, bool throwOnError)
		{
			Guard.ArgumentNotNull(() => context);

			string html = null;

			try
			{
				if (isPartial)
				{
					html = context.Controller.RenderPartialViewToString(viewName, model);
				}
				else
				{
					html = context.Controller.RenderViewToString(viewName, masterName, model);
				}
			}
			catch (Exception ex)
			{
				if (throwOnError)
				{
					throw ex;
				}
				else
				{
					return null;
				}
			}

			return html;
		}
	}

}
