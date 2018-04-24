
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using MyCommon.MyEnum;
using SpecDomain.BO;
using SpecDomain.Model;

namespace SpecDomain.BLL.EstItem
{
    public class MyEstItemCreate
    {
        public long EstItemID { get; private set; }

        private readonly int _estRevID;
        private readonly Product  _product;
        private readonly ProductSize  _template;


        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        public MyEstItemCreate(int estRevID, int productID, string  productName)
        {
            _estRevID = estRevID;
            _product = _db.Products.Find(productID);
            _template = _product.ProductCategory.ProductSizes .First( );

            EstItemID = CreateNewEstItem(productName );
            if (EstItemID >0)
            {

                var ct = new MyEstItemCreateSubTables(productID, EstItemID);
                ct.Create();

            }
        }

        private long CreateNewEstItem( string  productName)
        {
            try
            {
                var item = new EST_Item()
                               {
                                   EstRevID = _estRevID,
                                   SerialID = EstItemCommon.GetNewSerialID(_estRevID),
                                   EstItemNo =(short)EstItemCommon.GetNewNumber(_estRevID ),
                                   ProductID = _product.ProductID,
                                   ProductName = productName, 
                                   StatusID =(int)NEstItemStatus.New,
                                   EstPart =1,
                                   ItemOption = 1,
                                   IsFinalOption = true,
                                   Qty = 1,
                                   RequirementID = (int)NWorkorderRequirement.Installation,
                                   PositionID = 10,
                                   IsTemplateApplicable=true ,
                                   SizeRows=_template.SizeTotalRows ,
                                   EstimatorPrice =0,
                                   PriceA =0,
                                   PriceB =0,
                                   PriceExtra =0,
                               };

                //Create New EstItem
                _db.EST_Item.Add(item);
                _db.SaveChanges();

                return item.EstItemID;
            }
            catch (DbEntityValidationException dbEx)
            {
                var s = dbEx.Message;
                throw;
            }
        }

        
    }

    public class MyEstItemCreateSubTables
    {
        private readonly Product _product;
        private readonly ProductSize _template;
        private readonly long _estItemID;

        private readonly SpecificationDbEntities _db = new SpecificationDbEntities();

        public MyEstItemCreateSubTables(int productID, long estItemID)
        {
            _estItemID = estItemID;
            _product = _db.Products.Find(productID);
            _template = _product.ProductCategory.ProductSizes.First();
        }

        public void Create()
        {
            CreateNewSize();
            CreateNewSpecialFields();
        }

        private void CreateNewSize()
        {

            try
            {
                for (int i = 0; i < _template.SizeTotalRows; i++)
                {
                    var item = new EST_Item_Specification_Size()
                    {
                        EstItemID = _estItemID,
                        Pc = 0,
                        IsHeightEnabled = _template.IsHeightEnabled,
                        IsPcEnabled = _template.IsPcEnabled,
                        IsThicknessEnabled = _template.IsThicknessEnabled,
                        IsWidthEnabled = _template.IsWidthEnabled,

                        IsHeightMandatory  = _template.IsHeightMandatory,
                        IsPcMandatory = _template.IsPcEnabledMandatory,
                        IsThicknessMandatory = _template.IsThicknessMandatory,
                        IsWidthMandatory = _template.IsWidthMandatory,
                        IsValidated =true,
                    };

                    _db.EST_Item_Specification_Size.Add(item);
                }

                _db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var s = dbEx.Message;
                throw;
            }

        }

        private void CreateNewSpecialFields()
        {
         
            //Supposed to Have Fields 
            var configuedFields = _db.EST_Item_Specification_Template.Where(x => x.ProductID == _product.ProductID)
                                                          .OrderBy(x => x.OrderNumber)
                                                          .ToList();
            if (!configuedFields.Any())
            {
                //Disble Template
                var newValue = _db.EST_Item.Find(_estItemID);
                newValue.IsTemplateApplicable = false;
                _db.Entry(newValue).State = EntityState.Modified;
                _db.SaveChanges();
                return;
            }
            try
            {
                foreach (var field in configuedFields)
                {
                    var item = new EST_Item_Specification()
                    {
                        EstItemID = _estItemID,
                        OrderNumber = field.OrderNumber,
                        Title = field.Title,
                        OptionGroupID = field.OptionGroupID,
                        Contents = GetDefaultSpecialContents(field.DefaultValue),
                        IsMandatory = field.IsMandatory,
                    };
                    _db.EST_Item_Specification.Add(item);
                }
                _db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var s = dbEx.Message;
                throw;
            }
        }

        private string GetDefaultSpecialContents(int defaultOptionID)
        {
            var option = _db.EST_Item_Specification_Template_Option.Find(defaultOptionID);
            return option.OptionName;
        }

    }

}