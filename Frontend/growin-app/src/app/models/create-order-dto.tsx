interface CreateOrderDto {
  open: boolean;
  onClose: (success?: boolean) => void;
  product: string;
  productId: number; 
  maxQuantity: number;
}
