using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test1
{
    public class V 
    {
        /// <summary>
        /// aantal doses
        /// </summary>
        public int Dn { get; set; }
        /// <summary>
        /// datum van vaccinatie
        /// </summary>
        public string Dt { get; set; }
        /// <summary>
        /// doses nodig
        /// </summary>
        public int Sd { get; set; }


    }

    public class R
    {
        /// <summary>
        /// recovery date valid until
        /// </summary>
        public string Du { get; set; }
    }

    public class T
    {
        /// <summary>
        /// test result
        /// </summary>
        public string Tr { get; set; }
        /// <summary>
        /// test datum
        /// </summary>
        public string Sc { get; set; }
    }

    public class Nam
    {
        public string fn { get; set; }
        public string gn { get; set; }
        public string fnt { get; set; }
        public string gnt { get; set; }
    }


    public class JsonModel
    {
        public List<V> v { get; set; }
        public List<R> r { get; set; }
        public List<T> t { get; set; }
        public string dob { get; set; }
        public Nam nam { get; set; }
        public string ver { get; set; }

        public Boolean SafeAndVacc()
        {
            if (v != null) {
                int Needed = v[0].Sd / v[0].Dn;
                DateTime Vaccinated = DateTime.Parse(v[0].Dt);
                int Days = DateTime.Now.Subtract(Vaccinated).Days;
                if (Days > 14 && Needed == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            } 
            return false;
        }

        public Boolean TestandSafe()
        {
            if (t != null)
            {
                DateTime Tested = DateTime.Parse(t[0].Sc);
                int Days = DateTime.Now.Subtract(Tested).Days;
                string TestResult = t[0].Tr;
                if (Days > 3 && TestResult.Equals("260415000"))
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public Boolean Recovered()
        {
            if(r != null)
            {
                DateTime Isvalid = DateTime.Parse(r[0].Du);
                if (Isvalid.Subtract(DateTime.Now).Days > 1)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

