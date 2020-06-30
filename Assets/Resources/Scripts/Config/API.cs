using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 网络连接的接口
 */

public class API
{
    public static string LoginUrl = "https://www.xiaokucc.cn/merchant/login/login";
    public static string LoginUrl1 = "http://test.cheyuw.cn/api/merchant/auth/login";

    public static string RegisterUrl = "https://www.xiaokucc.cn/merchant/merchant/register";
    public static string GetMerchants = "https://www.xiaokucc.cn/merchant/merchant/getMerchants?pageSize=100&pageNumber=1";
    public static string ApplyLink = "https://www.xiaokucc.cn/merchant/merchant/linkMerchantApply";
    public static string GetApplyLink = "https://www.xiaokucc.cn/merchant/merchant/linkMerchantApplyList?pageSize=100&pageNumber=1";
    public static string HandApply = "https://www.xiaokucc.cn/merchant/merchant/handleLinkMerchantApply";
    public static string GetBrands = "https://www.xiaokucc.cn/merchant/merchant/getBrands";
    public static string GetCartLines = "https://www.xiaokucc.cn/merchant/merchant/getCartLines";
    public static string GetCartModels = "https://www.xiaokucc.cn/merchant/merchant/getCartModels";
    public static string AddCarSource = "https://www.xiaokucc.cn/merchant/merchant/addCartSource";
    public static string GetCarSource = "https://www.xiaokucc.cn/merchant/merchant/getMyCartSource";
    public static string UpdateCartSource = "https://www.xiaokucc.cn/merchant/merchant/updateCartSourceImport";
    public static string GetLinkMerchantCartSource = "https://www.xiaokucc.cn/merchant/merchant/getLinkMerchantCartSource?pageNumber=1&pageSize=1000";
    public static string GetSubAccounts = "https://www.xiaokucc.cn/merchant/merchant/getSubAccounts?pageSize=1000&pageNumber=1";
    public static string AddSubAccount = "https://www.xiaokucc.cn/merchant/merchant/addSubAccount";
    public static string UpdateSubAccountStatus = "https://www.xiaokucc.cn/merchant/merchant/updateSubAccountStatus";


    public static string _GetUserInfo = "https://service.xiaokucc.cn/api/merchant/user";
    public static string _GetUserInfo1 = "http://test.cheyuw.cn/api/merchant/user";

    public static string _GetMsgList = "https://service.xiaokucc.cn/api/merchant/order";
    public static string _GetMsgList1 = "http://test.cheyuw.cn/api/merchant/order";
    
    //2020.6.25新增
    public static string _GetMsgList23 = "http://op.cunguangmed.com:3000/mock/54/api/client/order/show";
    public static string PostYiJia = "http://op.cunguangmed.com:3000/mock/54/api/client/reply";//议价
    public static string GetCarInfo = "http://op.cunguangmed.com:3000/mock/54/api/client/cart/show";
    public static string PostConfirmYiJia = "http://op.cunguangmed.com:3000/mock/54/api/client/approve";//确认议价
    public static string PostCarsInfo = "http://test.cheyuw.cn/api/merchant/data/batch";
    public static string GetServerVehcle = "http://op.cunguangmed.com:3000/mock/54/api/client/line";//获取服务器车系列表
    public static string PostDeleteCarinfo = "http://test.cheyuw.cn//api/merchant/data/delete";//删除车辆信息
    public static string GetServerBrand = "http://op.cunguangmed.com:3000/mock/54/api/client/brand";//获取服务器品牌列表
    public static string _PostCarType = "http://test.cheyuw.cn/api/merchant/data/up_lines_models";//更新车型车系
    //2020.6.25新增

    public static string _PostReply = "https://service.xiaokucc.cn/api/merchant/reply";
    public static string _PostReply1 = "http://test.cheyuw.cn/api/merchant/reply";

    public static string _PostApprove = "https://service.xiaokucc.cn/api/merchant/approve";
    public static string _PostApprove1 = "http://test.cheyuw.cn/api/merchant/approve";


    public static string _GetCarList = "https://service.xiaokucc.cn/api/merchant/cart";
    public static string _GetCarList1 = "http://test.cheyuw.cn/api/merchant/cart";

    public static string _PostOfferPrice = "https://service.xiaokucc.cn/api/merchant/cart/offer";
    public static string _PostOfferPrice1 = "http://test.cheyuw.cn/api/merchant/data/batch";


    public static string _PostAddLoan = "https://service.xiaokucc.cn/api/merchant/cart_loan";
    public static string _PostAddLoan1 = "http://test.cheyuw.cn/api/merchant/cart_loan";

    public static string _PostDeleteLoan = "https://service.xiaokucc.cn/api/merchant/cart_loan/delete";
    public static string _PostDeleteLoan1 = "http://test.cheyuw.cn/api/merchant/cart_loan/delete";

    public static string _PostAddJingPin = "https://service.xiaokucc.cn/api/merchant/cart_boutique";
    public static string _PostAddJingPin1 = "http://test.cheyuw.cn/api/merchant/cart_boutique";

    public static string _PostDeleteJingPin = "https://service.xiaokucc.cn/api/merchant/cart_boutique/delete";
    public static string _PostDeleteJingPin1 = "http://test.cheyuw.cn/api/merchant/cart_boutique/delete";

   

}