using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace APIBookingAirTicket.controller
{
    public class BookingController : ApiController
    {
        DataClasses1DataContext db = new DataClasses1DataContext();

        [HttpGet]
        public List<DatCho> test()
        {
            return db.DatChos.ToList();
        }


        //service giữ chỗ chờ thanh toán, thời gian giữ 60phuts
        [HttpPost]
        public bool GiuCho(string key,string hang,string madatcho)
        {           
            //Kiem tra KEY có tồn tai và có phải là loại booking và còn Hạn Sử Dụng
            var k1 = db.KeyAPIs.FirstOrDefault(x => x.KeyID == key && x.Loai == "GiuCho" && x.NgayKetThuc >= DateTime.Today);
            if (k1 == null)     //key không thỏa điều kiện return false
                return false;
            else               //key thỏa điều kiện
            {
                DatCho datcho = new DatCho();

                try
                {
                    datcho.Hang = hang;
                    datcho.MaDatCho = madatcho;
                    datcho.TGChoBatDau = DateTime.Now;
                    datcho.TGChoKetThuc = DateTime.Now.AddMinutes(60);
                    datcho.TinhTrangThanhToan = 0;
                    db.DatChos.InsertOnSubmit(datcho);
                    db.SubmitChanges();//xác nhận thay đổi
                    return true;  //Giữ chỗ thành công
                }
                catch
                {
                    return false;
                }

            }
        }

        //service đặt chỗ
        [HttpPost]
        public bool DatCho(string key, string hang, string madatcho,string ten,int cmnd)
        {

            //Kiem tra KEY có tồn tai và có phải là loại Đặt Chỗ và còn Hạn Sử Dụng
            var k1 = db.KeyAPIs.FirstOrDefault(x => x.KeyID == key && x.Loai == "DatCho" && x.NgayKetThuc >= DateTime.Today);
            if (k1 == null)     //key không thỏa điều kiện return false
                return false;
            else               //key thỏa điều kiện
            {
                

                DatCho datcho = db.DatChos.FirstOrDefault(x => x.MaDatCho == madatcho && x.Hang == hang);
                if (datcho == null) //MaDatCho không tồn tại
                    return false;
                else if (datcho.TinhTrangThanhToan == 1)//Nếu TinhTrangThanhToan == 1 thì vé đã đặt -> return false
                    return false;
                else                               //Tình trạng thanh toán khác 1 -> vé chưa đặt -> set thời gian chờ thanh toán cho mã đặt chỗ là 60 phút
                {
                    try
                    {
                        datcho.Ten = ten;
                        datcho.CMND = cmnd;
                        datcho.TinhTrangThanhToan = 1;                       
                        db.SubmitChanges();//xác nhận thay đổi
                        return true;  //Đặt chỗ thành công
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }
    }
}
