using do_an.Models;

namespace do_an.ViewModels;

/// <summary>
/// ViewModel cho trang Store/Index, thay thế việc dùng ViewBag
/// </summary>
public class StoreIndexViewModel
{
    public List<SanPham> Products { get; set; } = new();
    public List<DanhMucSanPham> Categories { get; set; } = new();
    public List<SanPham> NoiBat { get; set; } = new();
    public List<DonThuoc> DonThuocs { get; set; } = new();
    public Dictionary<int, List<string>> DonThuocImages { get; set; } = new();
    public Dictionary<int, int> CountPerCategory { get; set; } = new();
    public int TotalAll { get; set; }
    public string? Search { get; set; }
    public int? DanhMucId { get; set; }
}
