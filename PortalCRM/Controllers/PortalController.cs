using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PortalCRM.Models;
using PortalCRM.Library;
using EarlyBoundTypes;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk.Metadata;
using PortalCRM.ViewModels;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;

namespace PortalCRM.Controllers
{
    public class PortalController : Controller
    {
        private PortalModelContext db = new PortalModelContext();
        private XrmServiceContext context = new ConnectionContext().XrmContext;

        // GET: Portal
        public ActionResult Index()
        {
            IQueryable<new_pyta> plyty = context.new_pytaSet.Select(row => row);

            List<PortalViewModel> pmList = new List<PortalViewModel>();

            foreach (new_pyta rec in plyty)
            {
                pmList.Add(new PortalViewModel {
                    Autor = rec.new_Autor,
                    Nazwa = rec.new_name,
                    Ranking = getOptionSetText("new_pyta", "new_ranking", (int)rec.new_Ranking),
                    Wydawca = rec.new_Wydawca.Name,
                    ID = rec.Id
                });
            }

            ViewBag.Plyty = plyty;

            return View(pmList);
        }

        // GET: Portal/Details/5
        public ActionResult Details(Guid id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            new_pyta plyta = (new_pyta)context.Retrieve("new_pyta", id, new ColumnSet(true));

            PortalViewModel pm = new PortalViewModel();

            pm.Autor = plyta.new_Autor;
            pm.ID = plyta.Id;
            pm.Nazwa = plyta.new_name;
            pm.Ranking = getOptionSetText("new_pyta", "new_ranking", plyta.new_Ranking.Value);
            pm.Wydawca = plyta.new_Wydawca.Name;

            if (pm == null)
            {
                return HttpNotFound();
            }
            return View(pm);
        }

        // GET: Portal/Create
        public ActionResult Create()
        {

            IQueryable<new_wydawca> wydawcy = context.new_wydawcaSet.Select(row => row);

            List<SelectListItem> wydawcyList = new List<SelectListItem>();
            
            foreach (var item in wydawcy)
            {
                wydawcyList.Add(new SelectListItem { Text = item.new_name, Value = item.new_wydawcaId.ToString() });
            }

            List<SelectListItem> rankingList = new List<SelectListItem>();

            OptionMetadataCollection omc = getOptionSetValues("new_pyta", "new_ranking");

            foreach (var item in omc)
            {
                string val = getOptionSetText("new_pyta", "new_ranking", (int)item.Value);
                int key = (int)item.Value;
                rankingList.Add(new SelectListItem { Text = val, Value = key.ToString() });

            }

            ViewBag.Ranking = rankingList;
            ViewBag.Wydawca = wydawcyList;

            return View();
        }

        // POST: Portal/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nazwa,Autor,Ranking,Wydawca")] PortalModel portalModel)
        {
            if (ModelState.IsValid)
            {
                

                new_pyta np = new new_pyta();

                
                np.new_Autor = portalModel.Autor;
                np.new_name = portalModel.Nazwa;
                
                np.new_Ranking = Int32.Parse(portalModel.Ranking);
                np.new_Wydawca = new EntityReference("new_wydawca", new Guid(portalModel.Wydawca));

                Guid result = context.Create(np);
                context.SaveChanges();

                
                return RedirectToAction("Index");
            }

            return View(portalModel);
        }

        // GET: Portal/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            new_pyta entity = (new_pyta)context.Retrieve("new_pyta", new Guid(id), new ColumnSet(true));

            List<SelectListItem> wydawcyList = new List<SelectListItem>();

            IQueryable<new_wydawca> wydawcy = context.new_wydawcaSet.Select(row => row);

            foreach (var item in wydawcy)
            {
                if (item.new_wydawcaId == entity.new_Wydawca.Id)
                {
                    wydawcyList.Add(new SelectListItem { Text = item.new_name, Value = item.new_wydawcaId.ToString(), Selected = true});
                }
                else
                {
                    wydawcyList.Add(new SelectListItem { Text = item.new_name, Value = item.new_wydawcaId.ToString() });
                }
                
            }

            List<SelectListItem> rankingList = new List<SelectListItem>();

            OptionMetadataCollection omc = getOptionSetValues("new_pyta", "new_ranking");

            foreach (var item in omc)
            {
                string val = getOptionSetText("new_pyta", "new_ranking", (int)item.Value);
                int key = (int)item.Value;

                if (key == entity.new_Ranking)
                {
                    rankingList.Add(new SelectListItem { Text = val, Value = key.ToString(), Selected = true});
                }
                else
                {
                    rankingList.Add(new SelectListItem { Text = val, Value = key.ToString() });
                }
                

            }

            

            PortalViewModel pvm = new PortalViewModel();

            pvm.Autor = entity.new_Autor;
            pvm.Nazwa = entity.new_name;
            

            if (entity == null)
            {
                return HttpNotFound();
            }

            ViewBag.Ranking = rankingList;
            ViewBag.Wydawca = wydawcyList;

            return View(pvm);
        }

        // POST: Portal/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nazwa,Autor,Ranking,Wydawca")] PortalViewModel pvm)
        {
            if (ModelState.IsValid)
            {

                new_pyta np = new new_pyta();
                np.new_Autor = pvm.Autor;
                np.new_name = pvm.Nazwa;
                np.new_Wydawca = new EntityReference("new_wydawca", new Guid(pvm.Wydawca));
                np.new_Ranking = Int32.Parse(pvm.Ranking);
                np.Id = pvm.ID;

                context.Update(np);
                context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(pvm);
        }

        // GET: Portal/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            new_pyta entity = (new_pyta)context.Retrieve("new_pyta", new Guid(id), new ColumnSet(true));

            PortalViewModel pvm = new PortalViewModel();
            pvm.Autor = entity.new_Autor;
            pvm.Nazwa = entity.new_name;
            pvm.ID = entity.Id;
            pvm.Ranking = getOptionSetText("new_pyta", "new_ranking", (int)entity.new_Ranking);
            pvm.Wydawca = entity.new_Wydawca.Name;
            
            return View(pvm);
        }

        // POST: Portal/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {

            context.Delete("new_pyta", new Guid(id));
            context.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public int getOptionSetValue(string entityName, string attributeName, string optionsetText)
        {
            int optionSetValue = 0;
            RetrieveAttributeRequest retrieveAttributeRequest = new RetrieveAttributeRequest();
            retrieveAttributeRequest.EntityLogicalName = entityName;
            retrieveAttributeRequest.LogicalName = attributeName;
            retrieveAttributeRequest.RetrieveAsIfPublished = true;

            RetrieveAttributeResponse retrieveAttributeResponse =
              (RetrieveAttributeResponse)context.Execute(retrieveAttributeRequest);
            PicklistAttributeMetadata picklistAttributeMetadata =
              (PicklistAttributeMetadata)retrieveAttributeResponse.AttributeMetadata;

            OptionSetMetadata optionsetMetadata = picklistAttributeMetadata.OptionSet;

            foreach (OptionMetadata optionMetadata in optionsetMetadata.Options)
            {
                if (optionMetadata.Label.UserLocalizedLabel.Label.ToString() == optionsetText)
                {
                    optionSetValue = optionMetadata.Value.Value;
                    return optionSetValue;
                }

            }
            return optionSetValue;
        }

        public string getOptionSetText(string entityName, string attributeName, int optionsetValue)
        {
            string optionsetText = string.Empty;
            RetrieveAttributeRequest retrieveAttributeRequest = new RetrieveAttributeRequest();
            retrieveAttributeRequest.EntityLogicalName = entityName;
            retrieveAttributeRequest.LogicalName = attributeName;
            retrieveAttributeRequest.RetrieveAsIfPublished = true;

            RetrieveAttributeResponse retrieveAttributeResponse =
              (RetrieveAttributeResponse)context.Execute(retrieveAttributeRequest);
            PicklistAttributeMetadata picklistAttributeMetadata =
              (PicklistAttributeMetadata)retrieveAttributeResponse.AttributeMetadata;

            OptionSetMetadata optionsetMetadata = picklistAttributeMetadata.OptionSet;

            foreach (OptionMetadata optionMetadata in optionsetMetadata.Options)
            {
                if (optionMetadata.Value == optionsetValue)
                {
                    optionsetText = optionMetadata.Label.UserLocalizedLabel.Label;
                    return optionsetText;
                }

            }
            return optionsetText;
        }

        public OptionMetadataCollection getOptionSetValues(string entityName, string attributeName)
        {
            
            RetrieveAttributeRequest retrieveAttributeRequest = new RetrieveAttributeRequest();
            retrieveAttributeRequest.EntityLogicalName = entityName;
            retrieveAttributeRequest.LogicalName = attributeName;
            retrieveAttributeRequest.RetrieveAsIfPublished = true;

            RetrieveAttributeResponse retrieveAttributeResponse =
              (RetrieveAttributeResponse)context.Execute(retrieveAttributeRequest);
            PicklistAttributeMetadata picklistAttributeMetadata =
              (PicklistAttributeMetadata)retrieveAttributeResponse.AttributeMetadata;

            OptionSetMetadata optionsetMetadata = picklistAttributeMetadata.OptionSet;


            return optionsetMetadata.Options;
        }

    }
}
