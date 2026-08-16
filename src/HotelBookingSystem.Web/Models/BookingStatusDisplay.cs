namespace HotelBookingSystem.Web.Models;

/// <summary>
/// يحوّل قيمة حالة الحجز (المخزّنة بالإنجليزية في قاعدة البيانات: Pending/Confirmed/Cancelled/Completed)
/// إلى تسمية عربية للعرض فقط. لا يغيّر القيمة الفعلية المستخدمة في المنطق أو في اسم صنف الشارة
/// (badge-status-pending...) حتى لا يتأثر أي شيء آخر في المشروع — تحويل عرضي بحت.
/// </summary>
public static class BookingStatusDisplay
{
    public static string ToArabic(string? status) => status switch
    {
        "Pending" => "قيد الانتظار",
        "Confirmed" => "مؤكد",
        "Cancelled" => "ملغي",
        "Completed" => "مكتملة",
        _ => status ?? string.Empty
    };
}

/// <summary>
/// يحوّل دور المستخدم (Visitor/RegisteredUser/Administrator) إلى تسمية عربية للعرض فقط.
/// </summary>
public static class UserRoleDisplay
{
    public static string ToArabic(string? role) => role switch
    {
        "Administrator" => "مدير النظام",
        "RegisteredUser" => "مستخدم مسجّل",
        "Visitor" => "زائر",
        _ => role ?? string.Empty
    };
}
