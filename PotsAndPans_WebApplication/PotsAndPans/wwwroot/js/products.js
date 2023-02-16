//var dataTable;

//$(document).ready(function () {
//    loadDataTable();
//});

//function loadDataTable() {
//    dataTable = $('#tblData').DataTable({
//        "ajax": {
//            "url": "/Admin/Product/GetAll"
//        },
//        "columns": [
//            { "data": "title", "width": "40%" },
//            { "data": "productNumber", "width": "25%" },
//            { "data": "price", "width": "15%" },
//            { "data": "category.name", "width": "15%" },
//            {
//                "data": "id",
//                "render": function (data) {
//                return `
//                    <div id="ProductBtn-Container" role="">
//                        <a id="editBtn" href="/Admin/Product/Upsert?id=${data}" class="btn  mx-2" style="background-color:black;color:white;"><i class="bi bi-pencil-square"></i>Edit</a>

//                        <a id="deleteBtn" onClick=Delete('/Admin/Product/Delete/+${data}') class="btn btn-danger mx-2"><i class="bi bi-trash-fill"></i>Delete</a>
//                    </div>   
//                    `
//                },
//                "width": "15%",
//            }
//        ]
//    });
//}

//function Delete(url) {
//    Swal.fire({
//        title: 'Are you sure?',
//        text: "You won't be able to revert this!",
//        icon: 'warning',
//        showCancelButton: true,
//        confirmButtonColor: '#000000',
//        cancelButtonColor: '#000000',
//        confirmButtonText: 'Yes, delete it!'
//    }).then((result) => {
//        if (result.isConfirmed) {
//            $.ajax({
//                url: url,
//                type: 'DELETE',
//                success: function (data) {
//                    if (data.success) {
//                        dataTable.ajax.reload();
//                        toastr.success(data.message);
//                    } else {
//                        toastr.error(data.message);

//                    }
//                }
//            })
//        }
//    })
//}





////* Loop through all dropdown buttons to toggle between hiding and showing its dropdown content - This allows the user to have multiple dropdowns without any conflict */
//var dropdown = document.getElementsByClassName("dropdown-btn");
//var i;

//for (i = 0; i < dropdown.length; i++) {
//    dropdown[i].addEventListener("click", function () {
//        this.classList.toggle("active");
//        var dropdownContent = this.nextElementSibling;
//        if (dropdownContent.style.display === "block") {
//            dropdownContent.style.display = "none";
//        } else {
//            dropdownContent.style.display = "block";
//        }
//    });
//}