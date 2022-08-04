(function () {
    app.controller('HomeInvoice', function ($scope, $http, $filter) {
        $scope.Sale = new Object();
        $scope.InvoiceCart = [];
        var init = function () {
            GetProducts();
        }; //end of init
        init(); //init is called

        function GetProducts() {
            $http.get('/Sale/GetAllProduct')
                .then(function (response) {
                    var data = response.data;
                    $scope.ProductList = data;
                });
        }
        $scope.AddNewRow = function () {
            $scope.InvoiceCart.push({ Id: null, UnitPrice: 0, Quantity: 1, LineTotal: 0 });
        }
        $scope.SetValueOfProduct = function (Id) {
            var dataObj = $filter('filter')($scope.ProductList, { Id: parseInt(Id) })[0];
            const index = $scope.InvoiceCart.findIndex((x) => x.Id === Id);
            $scope.InvoiceCart[index].UnitPrice = dataObj.SalesPrice;
            $scope.InvoiceCart[index].LineTotal = $scope.InvoiceCart[index].UnitPrice * $scope.InvoiceCart[index].Quantity;
        }
        $scope.OnChangeLineTotalSet = function (Id) {
            const index = $scope.InvoiceCart.findIndex((x) => x.Id === Id);
            $scope.InvoiceCart[index].LineTotal = $scope.InvoiceCart[index].UnitPrice * $scope.InvoiceCart[index].Quantity;
        }
        $scope.SubTotalCalculation = function () {
            $scope.Sale.Subtotal = 0;
            for (var i = 0; i < $scope.InvoiceCart.length; i++) {
                $scope.Sale.Subtotal = $scope.Sale.Subtotal + $scope.InvoiceCart[i].LineTotal;
            }
        }
        $scope.CalculateDiscount = function () {
            $scope.Sale.DiscountAmount = Math.round(($scope.Sale.Subtotal * $scope.Sale.DiscountParcentage) / 100);
        }
        $scope.CalculateSpecialDiscount = function () {
            $scope.Sale.SpecialDiscountAmount = Math.round((($scope.Sale.Subtotal - $scope.Sale.DiscountAmount) * $scope.Sale.SpecialDiscountParcentage) / 100);
        }
        $scope.CalculateVat = function () {
            $scope.Sale.VatAmount = Math.round(((($scope.Sale.Subtotal - $scope.Sale.DiscountAmount) - $scope.Sale.SpecialDiscountAmount) * $scope.Sale.VatParcentage) / 100);
            $scope.Sale.TotalAmount = Math.round((($scope.Sale.Subtotal - $scope.Sale.DiscountAmount) - $scope.Sale.SpecialDiscountAmount) + $scope.Sale.VatAmount);
            //$scope.Sale.TotalAmout = ($scope.Sale.Subtotal - $scope.Sale.DiscountAmount) + $scope.Sale.VatAmount;
        }
        $scope.RowDelete = function (index) {
            if (index > -1) {
                $scope.InvoiceCart.splice(index, 1);
            }
            $scope.SubTotalCalculation();
        }
        $scope.SaveInvoice = function () {
            $scope.Sale.OrderNo = $("#OrderNo").val();
            var data = JSON.stringify({
                sale: $scope.Sale, salesDetails: $scope.InvoiceCart
            });
            return $.ajax({
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                type: 'POST',
                url: "/Sale/SaveInvoiceSale",
                data: data,
                success: function (result) {
                    if (result.IsSuccess == true) {

                        //Reset();
                        alert("Save Success!");
                    }
                    else {
                        alert("Save failed!");
                    }
                },
                error: function () {
                    alert("Error!")
                }
            });
        }
    });
}).call(angular);