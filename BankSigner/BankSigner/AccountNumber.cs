namespace BankSigner
{
    using System;
    using System.Windows.Forms;

    public class AccountNumber
    {
        private string m_number;

        public string Number
        {
            get
            {
                return this.m_number;
            }
            set
            {
                bool flag = false;
                string str = "0123456789S-";
                this.m_number = value.Trim();
                foreach (char ch in this.m_number)
                {
                    if (!str.Contains(new string(ch, 1)) && !flag)
                    {
                        MessageBox.Show("Account numbers cannot contain the character '" + ch + "'.\nThey must be in the format (NUMBER)-S2-(NUMBER)-(MORE NUMBERS).", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        flag = true;
                    }
                }
                if (!this.m_number.Contains("-S2-") && !flag)
                {
                    MessageBox.Show("Account numbers must be in the format (NUMBER)-S2-(NUMBER)-(MORE NUMBERS).", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }
    }
}

