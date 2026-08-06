$(function () {
    // تفعيل DataTable.js على أي جدول يحمل الصنف data-table
    if ($.fn.DataTable) {
        $('.data-table').DataTable({
            language: {
                search: "بحث:",
                lengthMenu: "عرض _MENU_ سجل",
                info: "عرض _START_ إلى _END_ من أصل _TOTAL_ سجل",
                infoEmpty: "لا توجد سجلات",
                zeroRecords: "لا توجد نتائج مطابقة",
                paginate: { next: "التالي", previous: "السابق" }
            }
        });
    }

    // تبديل ظهور الشريط الجانبي على الشاشات الصغيرة
    $('#sidebarToggle').on('click', function () {
        $('.sidebar').toggleClass('show');
    });

    // تهيئة نافذة تأكيد الحذف الموحّدة: تُستدعى من زر يحمل data-delete-url و data-item-name
    $(document).on('click', '.js-delete-btn', function () {
        var url = $(this).data('delete-url');
        var name = $(this).data('item-name');
        $('#deleteConfirmForm').attr('action', url);
        $('#deleteItemName').text(name);
        var modal = new bootstrap.Modal(document.getElementById('deleteConfirmModal'));
        modal.show();
    });
});
