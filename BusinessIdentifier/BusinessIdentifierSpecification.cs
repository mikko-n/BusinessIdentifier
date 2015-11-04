using BusinessIdentifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessIdentifier
{
    public class BusinessIdentifierSpecification : ISpecification<string>
    {      
        /// <summary>
        /// multipliers used in check number calculations (from left to right)
        /// </summary>
        private static readonly int[] _weights = new int[] { 7, 9, 10, 5, 8, 4, 2 };

        /// <summary>
        /// Reason list
        /// </summary>
        private List<string> _reasons = new List<string>();

        #region ISpecification implementation

        public IEnumerable<String> ReasonsForDissatisfaction
        {
            get
            {                              
                return _reasons;
            }
        }

        public bool IsSatisfiedBy(string entity)
        {
            if (_reasons == null) { new List<string>(); }
            else { _reasons.Clear(); }

            if (String.IsNullOrEmpty(entity))
            {
                _reasons.Add(ReasonText.ERR_TyhjaSyote);
                return false;
            }

            // at first, try to preformat the business id
            string correctedBusinessId = CorrectEntityIfWrong(entity);

            // now, continue with formatted business id
            if (Regex.IsMatch(correctedBusinessId, @"^(\d{1,7}-{0,1}\d)$") == false)
            {
                _reasons.Add(string.Format(ReasonText.ERR_EiYTunnus, entity));
                return false;
            }

            if (Regex.IsMatch(correctedBusinessId, @"^([0]{1,7}-{0,1}[0])$") == true)
            {
                _reasons.Add(ReasonText.ERR_EiPelkkaaNollaa);
                return false;
            }

            // pick checksum and verify it
            int checkNum = int.Parse(correctedBusinessId[8].ToString());
            int calculatedCheckNum = CalculateCheckNum(correctedBusinessId.Substring(0, 7));

            if (calculatedCheckNum == int.MinValue)
            {
                _reasons.Add(ReasonText.ERR_ReminderOne);
                return false;
            }

            if (checkNum != calculatedCheckNum)
            {
                _reasons.Add(string.Format(ReasonText.ERR_ChecksumMismatch, checkNum, calculatedCheckNum));
                return false;
            }

            return true;
        }
   
        #endregion

        #region Helper methods

        /// <summary>
        /// Calculates the check number for business id number
        /// </summary>
        /// <param name="toBeChecked"></param>
        /// <returns>check number 0-9, int.MinValue if not valid</returns>
        private int CalculateCheckNum(string toBeChecked)
        {
            int current;
            int weightedSum = 0;

            // from left to right, weight with index
            for (int i = toBeChecked.Length-1, weightIdx = 6; i>= 0; i--, weightIdx--)
            {
                // formatException
                current = int.Parse(toBeChecked[i].ToString());

                weightedSum += (current * _weights[weightIdx]);
            }

            // calculate reminder
            int rem = weightedSum % 11;

            // there's no identifiers which produce reminder of 1
            if (rem == 1)
            {               
                return int.MinValue;
            }
            // reminder 0, check number = 0
            else if(rem == 0)
            {
                return 0;
            }
            // reminder 2-10, check number = 11-reminder
            else
            {
                return 11 - rem;
            }
        }
        
        /// <summary>
        /// Method to format input to official form of finnish business identification number,
        /// if not already
        /// </summary>
        /// <param name="entity">input</param>
        /// <returns>NNNNNNN-C, where numbers N and check number C are between 0-9 and divided by dash</returns>
        private string CorrectEntityIfWrong(string entity)
        {
            StringBuilder correctedEntity = new StringBuilder(entity);

            if (entity.IndexOf('-') == -1)
            {
                _reasons.Add(ReasonText.WARN_NoDash);
                // add dash, as we are nice people
                correctedEntity.Insert(entity.Length - 1, '-');
            }

            if (correctedEntity.Length < 9)
            {
                _reasons.Add(ReasonText.WARN_TooShort);
                // add leading zero for older bi's
                correctedEntity.Insert(0, "0", 9 - correctedEntity.Length);
            }
            
            return correctedEntity.ToString();
        }

        #endregion
    }
}
