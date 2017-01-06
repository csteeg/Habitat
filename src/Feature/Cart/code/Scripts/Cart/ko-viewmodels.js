;(function ($) {
    'use strict';

    window.features = window.features || {};
	window.features.cart = window.features.cart || {};
    window.features.cart.serviceUrl = window.features.cart.serviceUrl || '/services/v1/cart';
    window.features.cart.quantitySelect = window.features.cart.quantitySelect || function(selected) {
        var max = Math.max(selected + 2, 9),
            result = [];

        for (var i = 1; i <= max; i++)
            result.push(i);
        return result;
	}
	var cartFeature = window.features.cart;

	var camelize = function (str) {
	    return str.replace(/(?:^\w|[A-Z]|\b\w|\s+)/g, function(match, index) {
	        if (+match === 0) return ""; // or if (/\s+/.test(match)) for white spaces
	        return index == 0 ? match.toLowerCase() : match.toUpperCase();
	    });
	}

	var setProperties = function(properties, onObject, nonobservable) {
		if (properties && properties.length > 0) {
		    for (var i = 0; i < properties.length; i++) {
		        var key = camelize(properties[i].key);
				if (typeof (onObject[key]) == 'function') {
					onObject[key](onObject[properties[i].value]);
				} else {
					onObject[key] = nonobservable ? properties[i].value : ko.observable(properties[i].value);
				}
			}
		}
	}
	cartFeature.viewModels = {
        Cart: function () {
        	var self = this;

            self.lines = ko.observableArray([]);
            self.total = ko.observable();
            self.discountAdjustments = ko.observableArray([]);
            self.shipmentAdjustments = ko.observableArray([]);
            self.paymentAdjustments = ko.observableArray([]);
            self.addressIsValid = ko.observable(false);
            self.shippingIsValid = ko.observable(false);
            self.shipping = ko.observable();
            self.shippingOptions = ko.observableArray([]);
            self.payment = ko.observable();
            self.paymentOptions = ko.observableArray([]);
            self.externalId = ko.observable();
            self.subTotal = ko.observable();
            self.accountingCustomerPartyId = ko.observable();
            self.buyerCustomerPartyId = ko.observable();
            self.parties = ko.observableArray();

            self.totalProductCount = ko.pureComputed(function () {
                var total = 0,
                    lines = self.lines();

                if (typeof (lines) == 'undefined')
                    return 0;
            	for (var i = 0; i < lines.length; i++) {
            		total += lines[i].quantity();
            	}
            	return total;
            });

            self.taxPercentage = ko.computed(function() {
                if (self.total && self.total() && self.total().taxTotal && self.total().taxTotal.taxSubtotals) {
                    var taxSubtotals = self.total().taxTotal.taxSubtotals;
                    for (var i = 0; i < taxSubtotals.length; i++) {
                        if ((taxSubtotals[i].taxSubtotalType || "").toLowerCase() == "vat") {
                            if (taxSubtotals[i].percent < 1)
                                return taxSubtotals[i].percent * 100;
                            else
                                return taxSubtotals[i].percent;
                        }
                    }
                }

                return 0;
            }, self, { pure: true, deferEvaluation: true });

            self.getCartLine = function (id) {
                var lines = self.lines();
                if (typeof (lines) == 'undefined')
                    return;
                for (var i = 0; i < lines.length; i++) {
                    if (lines[i].externalCartLineId == id)
                        return lines[i];
                    var product = lines[i].product;
                    if (product && (product.productId == id || product.variantSku == id)) {
                        return lines[i];
                    }
                }
            };

            self.loadData = function (data) {
            	data = data || {};

            	if (data.lines) {
            	    for (var i = 0; i < data.lines.length; i++) {
            	        setProperties(data.lines[i].properties, data.lines[i], true);
                        if (data.lines[i].product) {
                            setProperties(data.lines[i].product.properties, data.lines[i].product, true);
                        }
                        data.lines[i]._quantity = data.lines[i].quantity;
                        data.lines[i].quantity = ko.computed({
                            read: function() {
                                return this._quantity;
                            },
                            write: function (value) {
                                if (this._quantity != parseInt(value)) {
                                    this._quantity = parseInt(value);
                                    self.updateQuantity(this.externalCartLineId, this._quantity);
                                }
                            },
                            owner: data.lines[i]
                        }, data.lines[i], { pure: true, deferEvaluation: true });
	                }
                }

                self.lines(data.lines);

                if (data.total) 
                    setProperties(data.total.properties, data.total, true);
				
                self.total(data.total);
				setProperties(data.properties, self);

                if (data.buyerCustomerParty && data.buyerCustomerParty.partyID) {
                    self.buyerCustomerPartyId(data.buyerCustomerParty.partyID);
                } else {
                    self.buyerCustomerPartyId(false);
                }

                if (data.accountingCustomerParty && data.accountingCustomerParty.partyID) {
                    self.accountingCustomerPartyId(data.accountingCustomerParty.partyID);
                } else {
                    self.accountingCustomerPartyId(false);
                }

                if (data.parties) {
                    for (var i = 0; i < data.parties.length; i++) {
                        setProperties(data.parties[i].properties, data.parties[i], true);
                    }
                }

                self.parties(data.parties);
                self.externalId(data.externalId);

                if (data.adjustments) {
                    for (var i = 0; i < data.adjustments.length; i++) {
                        setProperties(data.adjustments[i].properties, data.adjustments[i], true);
                    }
					self.discountAdjustments($.grep(data.adjustments, function (item, index) {
						return (item.type == "discount");
					}));
					self.shipmentAdjustments($.grep(data.adjustments, function (item, index) {
						return (item.type == "shipment");
					}));
					self.paymentAdjustments($.grep(data.adjustments, function (item, index) {
						return (item.type == "payment");
					}));
                }


                if (data.shipping && data.shipping.length > 0) {
                    setProperties(data.shipping[0].properties, data.shipping[0], true);
                    self.shipping(data.shipping[0]);
                } else {
                    self.shipping(null);
                }

                if ($('.apply-shipping-binding').length > 0 && self.parties() && self.parties().length > 0 &&
                    (!self.shippingOptions() || self.shippingOptions().length == 0)) {
                    self.loadShippingOptions();
                }

            }

            self.load = function(callback) {
                $.getJSON(cartFeature.serviceUrl,
                {
                    includeStockInformation: $('.apply-stock-binding').length > 0
                }, function (data) {
                    self.loadData(data);
					if (callback) {
					    callback(self);
					}
            	});
            }

            self.addProduct = function(productId, quantity) {
                $.post(cartFeature.serviceUrl,
                    {
                        productId: productId,
                        quantity: parseInt(quantity) || 0
                    },
                    function(data) {
                        self.loadData(data);
                        $(window).trigger('addedToCart', [productId, quantity, self]);
                    });
            },
            self.updateQuantity = function(cartLineId, newQuantity) {
                var cartLine = self.getCartLine(cartLineId),
                    oldQuantity = cartLine.quantity,
                    productId = cartLine.product ? cartLine.product.productId : 'unknown';
                $.ajax({
                    url: cartFeature.serviceUrl,
                    type: 'PUT',
                    data: {
                        cartLineId: cartLineId,
                        quantity: newQuantity
                    },
                    success: function(result) {
                        self.loadData(result);
                        $(window).trigger('updatedCart', [productId, oldQuantity, newQuantity, cartLine, self]);
                    }
                });

            },
            self.removeLine = function(cartLineId) {
                var cartLine = self.getCartLine(cartLineId),
                    productId = cartLine.product ? cartLine.product.productId : 'unknown';

                $('#' + cartLineId).slideUp('fast', function () {
                    $.ajax({
                    	url: cartFeature.serviceUrl,
                    	type: 'DELETE',
                    	data: { '': cartLineId },
                    	success: function (result) {
                    		self.loadData(result);
                    		$(window).trigger('removedFromCart', [productId, cartLine, self]);
                    	}
                    });
                });                    
            };

            self.getStockInfo = function(productId) {
                if (!self.stockInformation || !self.stockInformation())
                    return 0;

                for (var i = 0; i < self.stockInformation().length ; i++) {
                    if (self.stockInformation()[i].product && (self.stockInformation()[i].product.productId == productId)) {
                        return self.stockInformation()[i].count;
                    }
                }
                return 0;
            };

            self.allArticlesInStock = ko.computed(function() {
                var lines = self.lines();

                if (!lines || lines.length == 0 || !self.stockInformation || !self.stockInformation())
                    return false;

                for (var i = 0; i < lines.length; i++) {
                    if (self.getStockInfo(lines[i].product.variantSku) < lines[i].quantity()) {
                        return false;
                    }
                }

                return true;
            }, self, { pure: true, deferEvaluation: true });

            self.getParty = function (partyId) {
                if (self.parties && self.parties().length) {
                    for (var i = 0; i < self.parties().length; i++) {
                        if (self.parties()[i].partyId == partyId)
                            return self.parties()[i];
                    }
                }
                return {
                    "partyId": "",
                    "firstName": "",
                    "lastName": "",
                    "email": "",
                    "company": "",
                    "address1": "",
                    "address2": "",
                    "zipPostalCode": "",
                    "city": "",
                    "state": "",
                    "country": "",
                    "phoneNumber": "",
                    "isPrimary": "",
                    "externalId": "",
                    "properties": []};
            }

            self.updateAddress = function(form) {
                if (!self.externalId || !self.externalId())
                    return;
                var form = $(form);
                if (self.validateForm(form)) {
                    $.ajax({
                        url: cartFeature.serviceUrl + '/' + self.externalId() + "/address",
                        type: 'PUT',
                        data: form.serialize(),
                        success: function (result) {
							self.loadData(result);
                            $(window).trigger('updatedAddress', [self]);
                            self.addressIsValid(true);
                        }
                    });
				}
            }

            self.validateForm = function (form) {
                var form = $(form);
                return (typeof (form.valid) == 'undefined' || form.valid());
            }

            self.validateAllForms = function () {
                for (var i = 0; i < arguments.length; i++) {
                    console.log('validating form: ' + arguments[i]);
                    if (self.validateForm(arguments[i]) === false) {
                        console.log('Form ' + arguments[i] + ' is invalid!');
                        return false;
                    }
                }
                return true;
            }

            self.loadShippingOptions = function() {
                $.ajax({
                    url: cartFeature.serviceUrl + '/' + self.externalId() + "/shipping",
                    type: 'GET',
                    success: function(result) {
                        if (result && result.length > 0) {
                            for (var i = 0; i < result.length; i++) {
                                setProperties(result[i].properties, result[i], true);
                            }
                        }
                        self.shippingOptions(result);
                        if (!self.shipping() && self.shippingOptions().length > 0) {
                            self.updateShippingMethod(self.shippingOptions()[0].externalId);
                        }
                        $(window).trigger('shippingOptionsLoaded', [self.shippingOptions(), self]);
                    }
                });
            };

            self.updateShippingMethod = function(shippingId) {
                $.ajax({
                    url: cartFeature.serviceUrl + '/' + self.externalId() + "/shipping",
                    type: 'PUT',
                    data: { '': shippingId },
                    success: function (result) {
                        self.loadData(result);
                        $(window).trigger('updatedShippingMethod', [self]);
                    }
                });
            };
            self.goToPaymentStep = function (form) {
                var form = $(form);
                if (self.validateForm(form)) {
                    self.shippingIsValid(true);
                }
            }

            return false;
        }

    };

    var init = function() {
		if (typeof (ko) == 'undefined') {
			console.warn("Cart feature requires knockout to be loaded!");
			return;
        }

        // Here's a custom Knockout binding that makes elements shown/hidden via jQuery's slideDown()/slideUp() methods
        // Could be stored in a separate utility library
        ko.bindingHandlers.slideVisible = {
            init: function (element, valueAccessor) {
                // Initially set the element to be instantly visible/hidden depending on the value
                var value = valueAccessor();
                $(element).toggle(ko.unwrap(value)); // Use "unwrapObservable" so we can handle values that may or may not be observable
            },
            update: function (element, valueAccessor) {
                // Whenever the value subsequently changes, slowly fade the element in or out
                var value = valueAccessor();
                ko.unwrap(value) ? $(element).slideDown() : $(element).slideUp();
            }
        };

		var cart = cartFeature.current = new cartFeature.viewModels.Cart();
        cart.load(function (loadedCart) { $('.apply-cart-binding').each(function() {
             ko.applyBindings(loadedCart, this);
        })});

        $(window).on('addToCart', function(event, productId, quantity) {
            cart.addProduct(productId, quantity);
        });
	}

	$(document).ready(init);

})(jQuery);
