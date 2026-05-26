$urls = @(
    'http://localhost:5270',
    'http://localhost:5270/Account/Login',
    'http://localhost:5270/SanPham/Index',
    'http://localhost:5270/PhieuNhapHang/Index',
    'http://localhost:5270/NhaCungCap/Index',
    'http://localhost:5270/DonHangPos/Index',
    'http://localhost:5270/DonHangOnline/Index',
    'http://localhost:5270/LoThuoc/Index',
    'http://localhost:5270/Voucher/Index',
    'http://localhost:5270/BaoCao/Index',
    'http://localhost:5270/health'
)

foreach ($url in $urls) {
    try {
        $r = Invoke-WebRequest -Uri $url -UseBasicParsing -MaximumRedirection 5
        Write-Host "$($r.StatusCode) OK  => $url"
    } catch {
        $code = $_.Exception.Response.StatusCode.value__
        Write-Host "$code ERR => $url"
    }
}
