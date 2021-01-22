using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OdataAPI.ERPNext.Customer
{
    public class CustomerModel
    {
        public CustomerRoot[] data { get; set; }
    }

    public class CustomerRoot
    {
        [Key]
        public string name { get; set; }
        public string creation { get; set; }
        public string modified { get; set; }
        public string modified_by { get; set; }
        public string owner { get; set; }
        public int docstatus { get; set; }
        public object parent { get; set; }
        public object parentfield { get; set; }
        public object parenttype { get; set; }
        public int idx { get; set; }
        public string naming_series { get; set; }
        public object salutation { get; set; }
        public string customer_name { get; set; }
        public object gender { get; set; }
        public string customer_type { get; set; }
        public object default_bank_account { get; set; }
        public object lead_name { get; set; }
        public object image { get; set; }
        public object account_manager { get; set; }
        public string customer_group { get; set; }
        public string territory { get; set; }
        public object tax_id { get; set; }
        public object tax_category { get; set; }
        public int so_required { get; set; }
        public int dn_required { get; set; }
        public int disabled { get; set; }
        public int is_internal_customer { get; set; }
        public object represents_company { get; set; }
        public object default_currency { get; set; }
        public object default_price_list { get; set; }
        public string language { get; set; }
        public object website { get; set; }
        public string customer_primary_contact { get; set; }
        public string mobile_no { get; set; }
        public string email_id { get; set; }
        public object customer_primary_address { get; set; }
        public object primary_address { get; set; }
        public object payment_terms { get; set; }
        public object customer_details { get; set; }
        public object market_segment { get; set; }
        public object industry { get; set; }
        public int is_frozen { get; set; }
        public object loyalty_program { get; set; }
        public object loyalty_program_tier { get; set; }
        public object default_sales_partner { get; set; }
        public double default_commission_rate { get; set; }
        public object customer_pos_id { get; set; }
        public object _user_tags { get; set; }
        public object _comments { get; set; }
        public object _assign { get; set; }
        public object _liked_by { get; set; }
    }


}
