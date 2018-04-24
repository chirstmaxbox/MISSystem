using System;
using System.ComponentModel.DataAnnotations;

using System.Data.Entity;
using System.Linq;
using MyCommon;
using SpecDomain.BO;
using SpecDomain.Model;

namespace SpecDomain.BLL.Material
{
  public class MaterialDeleteVm
  {
      public MyValidation DeleteValidation { get; set; }
      private readonly SpecificationDbEntities _db = new SpecificationDbEntities();
      private readonly SpecDomain.Model.Material _material;

      public MaterialDeleteVm(int materialID)
      {
          _material = _db.Materials.FirstOrDefault(x=>x.MaterialID ==materialID);
          DeleteValidation = GetValidateResult();
      }


      public void Delete()
      {
          if (!DeleteValidation.IsValid) return;

          var prices = _material.MaterialPrices.ToList();
          foreach (var price in prices)
          {
              _db.Entry(price).State = EntityState.Deleted;
          }
          _db.SaveChanges();
          
          _db.Entry(_material).State = EntityState.Deleted;
          _db.SaveChanges();

      }


      private MyValidation GetValidateResult()
      {
          var val = new MyValidation();

          if (_material == null)
          {
              val.IsValid = false;
              var err01 = new MyKeyValuePair()
                              {
                                  Key = 1,
                                  Value = "Request Failed, Can't find Specified Materil",
                              };
              val.ErrorMessages.Add(err01);
              return val;
          }


          //Table JobCosting Template
          var tempalteDetails = _db.EST_Cost_Template_Detail.Where(x => x.DbItemID == (int) _material.MaterialID &
                                                                        (x.TypeID == (int) NEstCostType.ShopMaterail
                                                                         ||
                                                                         x.TypeID ==
                                                                         (int) NEstCostType.InstallationMaterail)
              ).ToList();
          if (tempalteDetails.Any())
          {
              val.IsValid = false;
              var err02 = new MyKeyValuePair()
                              {
                                  Key = 2,
                                  Value = "Request Failed, This material has been used in Estimation Template",
                              };
              val.ErrorMessages.Add(err02);

          }


          //Table JobCostingTransaction
          var transactions = _db.JobCostingTransactions.Where(x => x.MaterialID == (int) _material.MaterialID).ToList();
          if (transactions.Any())
          {
              //NJobCostingTransactionTypeID.DirectInput

              transactions = transactions.Where(x => x.TransactionType == 0 ).ToList();
              if (transactions.Any())
              {
                  val.IsValid = false;
                  var err03 = new MyKeyValuePair()
                  {
                      Key = 3,
                      Value = "Request Failed, This material has been used in Job Costing",
                  };
                  val.ErrorMessages.Add(err03);                  
              }
          }


          //Table JobCostingMaterialTemplateDetail
          var est =_db.JobCostingMaterialTemplateDetails.Where(x => x.MaterialID == (int) _material.MaterialID).ToList();
          if (transactions.Any())
          {
              val.IsValid = false;
              var err04 = new MyKeyValuePair()
                              {
                                  Key = 4,
                                  Value = "Request Failed, This material has been used in Job Costing Template",
                              };
              val.ErrorMessages.Add(err04);

          }

          return val;

      }

  }


}