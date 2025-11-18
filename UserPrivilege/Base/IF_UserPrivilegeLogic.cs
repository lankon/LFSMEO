using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserPrivilege.Base
{
    public interface IF_UserPrivilegeLogic
    {
        void GetDataGridInfo(List<Dictionary<string, object>> data);
        void SaveAccountPassword();
        eUserLevel CheckUserPrivilege(string input_account, string input_password);
        void SetCurLevel(eUserLevel level);
        eUserLevel GetCurLevel();
        bool AtLeastEng();
    }
}
