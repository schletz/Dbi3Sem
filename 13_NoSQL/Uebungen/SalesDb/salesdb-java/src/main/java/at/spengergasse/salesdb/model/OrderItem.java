package at.spengergasse.salesdb.model;

import java.math.BigDecimal;

public record OrderItem(Product product, int quantity, BigDecimal itemPrice) {
}