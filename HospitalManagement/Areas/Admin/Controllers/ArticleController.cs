using BELibrary.Core.Entity;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;
using BELibrary.Utils;
using HospitalManagement.Areas.Admin.Authorization;
using PagedList;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalManagement.Areas.Admin.Controllers
{
    public class ArticleController : BaseController
    {
        private const string KeyElement = "Bài viết";  // Xác định tên của phần tử cho tiêu đề

        // GET: Admin/Article
        public ActionResult Index()
        {
            ViewBag.Feature = "Danh sách";  // Tiêu đề cho phần này
            ViewBag.Element = KeyElement;  // Đưa tên phần tử vào viewbag
            return RedirectToAction("Search");  // Chuyển hướng tới trang tìm kiếm
        }

        // Phương thức tìm kiếm bài viết
        public ActionResult Search(string query, int? page)
        {
            ViewBag.Feature = "Danh sách";  // Tiêu đề
            ViewBag.Element = KeyElement;  // Tên phần tử
            ViewBag.Host = (Request.Url == null ? "" : Request.Url.Host);  // Đường dẫn host

            var watch = System.Diagnostics.Stopwatch.StartNew();  // Bắt đầu đo thời gian

            // Nếu query rỗng, gán giá trị null
            if (query == "")
            {
                query = null;
            }

            ViewBag.QueryData = query;  // Truyền query vào viewbag
            var pageNumber = (page ?? 1);  // Xác định số trang hiện tại, mặc định là 1
            const int pageSize = 5;  // Kích thước mỗi trang (5 bài viết)

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var listData = workScope.Articles.Query(x => !x.IsDelete).OrderByDescending(x => x.ModifiedDate).ToList();  // Lấy tất cả bài viết chưa bị xóa và sắp xếp theo ngày sửa đổi

                double elapsedMs = 0;
                if (query == null)  // Nếu không có query, chỉ lấy tất cả bài viết
                {
                    ViewBag.Total = listData.Count();  // Đếm tổng số bài viết
                    watch.Stop();  // Dừng đồng hồ

                    elapsedMs = (double)watch.ElapsedMilliseconds / 1000;  // Tính thời gian thực hiện
                    ViewBag.RequestTime = elapsedMs;  // Truyền thời gian vào viewbag
                    return View(listData.ToPagedList(pageNumber, pageSize));  // Trả về danh sách bài viết phân trang
                }

                // Tìm kiếm bài viết theo tiêu đề, mô tả hoặc nội dung
                var q = (from mt in listData
                         where (!string.IsNullOrEmpty(query) &&
                                (mt.Title.ToLower().Contains(query.ToLower())
                                 || !string.IsNullOrEmpty(mt.Description) && mt.Description.ToLower().Contains(query.ToLower())
                                 || !string.IsNullOrEmpty(mt.Content) && mt.Content.ToLower().Contains(query.ToLower())))
                         select mt).AsQueryable();

                ViewBag.Total = q.Count();  // Đếm số bài viết sau khi lọc
                watch.Stop();  // Dừng đồng hồ

                elapsedMs = (double)watch.ElapsedMilliseconds / 1000;  // Tính thời gian thực hiện
                ViewBag.RequestTime = elapsedMs;  // Truyền thời gian vào viewbag
                return View(q.ToPagedList(pageNumber, pageSize));  // Trả về kết quả tìm kiếm phân trang
            }
        }

        // Hiển thị chi tiết bài viết
        public ActionResult Detail(Guid id)
        {
            ViewBag.Element = KeyElement;  // Tiêu đề phần tử
            ViewBag.Feature = "Chi tiết";  // Tiêu đề phần chi tiết
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var regimen = workScope.Articles.FirstOrDefault(x => x.Id == id);  // Lấy bài viết theo ID
                if (regimen != null)
                {
                    return View(regimen);  // Trả về view chi tiết bài viết
                }
                else
                {
                    return RedirectToAction("Create", "Article");  // Nếu không tìm thấy bài viết, chuyển đến trang tạo bài viết mới
                }
            }
        }

        // Phương thức tạo bài viết mới
        [Permission(Role = 1)]  // Kiểm tra quyền người dùng
        public ActionResult Create()
        {
            ViewBag.Feature = "Thêm mới";  // Tiêu đề phần tạo mới
            ViewBag.Element = KeyElement;  // Tiêu đề phần tử
            ViewBag.isEdit = false;  // Đánh dấu là không phải chỉnh sửa
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                return View();  // Trả về form tạo bài viết mới
            }
        }

        // Phương thức cập nhật bài viết
        [Permission(Role = 1)]  // Kiểm tra quyền người dùng
        public ActionResult Update(Guid id)
        {
            ViewBag.isEdit = true;  // Đánh dấu là chỉnh sửa
            ViewBag.Feature = "Cập nhật";  // Tiêu đề phần cập nhật
            ViewBag.Element = KeyElement;  // Tiêu đề phần tử
            if (Request.Url != null)
            {
                ViewBag.BaseURL = string.Join("", Request.Url.Segments.Take(Request.Url.Segments.Length - 1));  // Lấy URL cơ bản
            }

            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var patient = workScope.Articles
                    .FirstOrDefault(x => x.Id == id && !x.IsDelete);  // Lấy bài viết cần cập nhật

                if (patient != null)
                {
                    return View("Create", patient);  // Trả về form cập nhật với dữ liệu hiện có
                }
                else
                {
                    return RedirectToAction("Create", "Article");  // Nếu không tìm thấy, chuyển tới trang tạo bài viết mới
                }
            }
        }

        // Phương thức trả về dữ liệu JSON cho autocomplete
        [HttpGet]
        public JsonResult GetJson(string query)
        {
            using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
            {
                var regimens = workScope.Articles.Query(x => x.Title.Contains(query))  // Lọc bài viết theo tiêu đề
                    .Select(x => new
                    {
                        value = x.Title,
                        data = x.Id
                    }).ToList();

                return Json(new
                {
                    suggestions = regimens  // Trả về danh sách bài viết dưới dạng JSON
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // Phương thức thêm mới hoặc cập nhật bài viết
        [Permission(Role = 1)]  // Kiểm tra quyền người dùng
        [HttpPost, ValidateInput(false)]  // Xử lý POST và không kiểm tra đầu vào HTML
        public JsonResult CreateOrEdit(Article input, bool isEdit)
        {
            try
            {
                var user = CookiesManage.GetUser();  // Lấy thông tin người dùng hiện tại
                if (isEdit)  // Nếu là cập nhật bài viết
                {
                    using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                    {
                        var elm = workScope.Articles.FirstOrDefault(x => x.Id == input.Id);  // Tìm bài viết theo ID
                        if (elm != null)  // Nếu bài viết tồn tại
                        {
                            input.CreatedDate = elm.CreatedDate;
                            input.ModifiedDate = DateTime.Now;

                            input.CreatedBy = elm.CreatedBy;
                            input.ModifiedBy = user.FullName;

                            elm = input;  // Cập nhật bài viết
                            elm.Content = elm.Content.Replace("§", "o");  // Thay thế ký tự đặc biệt

                            workScope.Articles.Put(elm, elm.Id);  // Lưu bài viết vào DB
                            workScope.Complete();  // Hoàn tất giao dịch
                            return Json(new
                            {
                                status = true,
                                mess = "Cập nhật thành công ",
                            });
                        }

                        return Json(new { status = false, mess = "Không tồn tại " });  // Nếu bài viết không tồn tại
                    }
                }

                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    input.Id = Guid.NewGuid();  // Tạo ID mới cho bài viết
                    input.CreatedDate = DateTime.Now;
                    input.ModifiedDate = DateTime.Now;

                    input.CreatedBy = user.FullName;
                    input.ModifiedBy = user.FullName;
                    workScope.Articles.Add(input);  // Thêm bài viết vào DB
                    workScope.Complete();  // Hoàn tất giao dịch
                }

                return Json(new
                {
                    status = true,
                    mess = "Thêm thành công ",
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    mess = "Có lỗi xảy ra: " + ex.Message
                });
            }
        }

        // Phương thức upload file ảnh
        [Permission(Role = 1)]  // Kiểm tra quyền người dùng
        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            try
            {
                if (upload?.FileName != null)
                {
                    if (upload.ContentLength >= FileKey.MaxLength)  // Kiểm tra kích thước file
                    {
                        return Json(new { status = false, mess = "File Max Length" });
                    }
                    var splitFilename = upload.FileName.Split('.');  // Tách phần mở rộng
                    if (splitFilename.Length > 1)
                    {
                        var fileExt = splitFilename[splitFilename.Length - 1];

                        // Kiểm tra phần mở rộng file
                        if (FileKey.FileExtensionApprove().Any(x => x == fileExt))
                        {
                            var now = DateTime.Now;
                            var yearName = now.ToString("yyyy");
                            var monthName = now.ToString("MMMM");
                            var dayName = now.ToString("dd-MM-yyyy");

                            var folder = Path.Combine(Server.MapPath("~/FileUploads/images/"),
                                Path.Combine(yearName,
                                    Path.Combine(monthName,
                                        dayName)));  // Tạo đường dẫn thư mục theo năm, tháng, ngày
                            var createFolder = Directory.CreateDirectory(folder);  // Tạo thư mục nếu chưa tồn tại

                            var slugName = StringHelper.ConvertToAlias(upload.FileName);  // Chuyển tên file thành dạng slug
                            var fileName = slugName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + fileExt;
                            var path = Path.Combine(createFolder.FullName, fileName);
                            upload.SaveAs(path);  // Lưu file vào server

                            return Json(new { status = true, mess = "Cập nhật thành công", url = $"/FileUploads/images/{yearName}/{monthName}/{dayName}/{fileName}" });  // Trả về đường dẫn file
                        }
                        return Json(new { status = false, mess = "FileExtensionReject" });
                    }

                    return Json(new { status = false, mess = "FileExtensionReject" });
                }
                return Json(new { status = false, mess = "Cập nhật không thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, mess = "Cập nhật không thành công", ex });
            }
        }

        // Phương thức xóa bài viết
        [Permission(Role = 1)]  // Kiểm tra quyền người dùng
        [HttpPost]
        public JsonResult Del(Guid id)
        {
            try
            {
                using (var workScope = new UnitOfWork(new HospitalManagementDbContext()))
                {
                    var elm = workScope.Articles.Get(id);  // Lấy bài viết theo ID
                    if (elm != null)  // Nếu bài viết tồn tại
                    {
                        elm.IsDelete = true;  // Đánh dấu là xóa mềm
                        workScope.Articles.Put(elm, elm.Id);  // Lưu lại
                        workScope.Complete();  // Hoàn tất giao dịch
                        return Json(new { status = true, mess = "Xóa thành công " });
                    }
                    else
                    {
                        return Json(new { status = false, mess = "Không tồn tại " });  // Nếu không tìm thấy bài viết
                    }
                }
            }
            catch
            {
                return Json(new { status = false, mess = "Thất bại" });
            }
        }
    }
}
