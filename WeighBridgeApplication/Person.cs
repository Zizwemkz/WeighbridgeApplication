using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace WeighBridgeApplication
{
    public class Person
    {
        // The two items below will be displayed by DataLayoutControl 
        // in a borderless and titleless Name group
        [Display(GroupName = "<Name>", Name = "Last name")]
        public string LastName { get; set; }
        [Display(GroupName = "<Name>", Name = "First name", Order = 0)]
        public string FirstName { get; set; }

        //The four items below will go to a Contact tab within tabbed Tabs group.
        [Display(GroupName = "{Tabs}/Contact", Order = 2),
            DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [Display(GroupName = "{Tabs}/Contact", Order = 4)]
        public string Email { get; set; }
        //The two items below will go to the Address group within the Contact tab.
        [Display(GroupName = "{Tabs}/Contact/Address", ShortName = "")]
        public string AddressLine1 { get; set; }
        [Display(GroupName = "{Tabs}/Contact/Address", ShortName = "")]
        public string AddressLine2 { get; set; }

        //The two items below will go to the horizontally oriented Personal group.
        [Display(GroupName = "Personal-", Name = "Birth date")]
        public DateTime BirthDate { get; set; }
        [Display(GroupName = "Personal-", Order = 3)]
        public Gender Gender { get; set; }

        //The four items below will go to the Job tab of the tabbed Tabs group
        [Display(GroupName = "{Tabs}/Job", Order = 6)]
        public string Group { get; set; }
        [Display(GroupName = "{Tabs}/Job", Name = "Hire date")]
        public DateTime HireDate { get; set; }
        [Display(GroupName = "{Tabs}/Job"), DataType(DataType.Currency)]
        public decimal Salary { get; set; }
        [Display(GroupName = "{Tabs}/Job", Order = 7)]
        public string Title { get; set; }
    }

    public enum Gender { Male, Female }
}
