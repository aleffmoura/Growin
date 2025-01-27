import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';
import * as React from 'react';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import Button from '@mui/material/Button';
import CreateOrderFormDialog from '../orders/create-order';
import { ProductResumeViewModel } from '@/app/models/product-resume-view-model';
import { useEffect, useState } from 'react';
import { Box, CircularProgress } from '@mui/material';
import { fetchProducts } from '@/app/utils/api';

export default function ProductsComponent() {
  const [open, setOpen] = useState(false);
  const [products, setProducts] = useState<ProductResumeViewModel[]>([]);
  const [loading, setLoading] = useState(true);

  const [selectedProduct, setSelectedProduct] = useState<{
    id: number,
    name: string;
    quantity: number;
  } | null>(null);

  const handleOpenDialog = (productId: number, productName: string, productQuantity: number) => {
    setSelectedProduct({ id: productId, name: productName, quantity: productQuantity });
    setOpen(true);
  };

  const handleCloseDialog = () => {
    setOpen(false);
    setSelectedProduct(null);
  };

  useEffect(() => {
    async function loadProducts() {
      const response = await fetchProducts();
      setProducts(response);
      setLoading(false);
    }
  
    loadProducts();
  }, []);
  
  if (loading) return (<Box sx={{ display: 'flex' }}><CircularProgress /></Box>);

  return (
    <>
      <TableContainer component={Paper}>
        <Table sx={{ minWidth: 550 }} aria-label="simple table">
          <TableHead>
            <TableRow>
              <TableCell>Id</TableCell>
              <TableCell align="right">Nome</TableCell>
              <TableCell align="right">Quantidade em estoque</TableCell>
              <TableCell align="right"></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {products.map((product, index) => (
              <TableRow key={`${product.id}-${index}`} sx={{ '&:last-child td, &:last-child th': { border: 0 } }}>
                <TableCell component="th" scope="row">
                  {product.id}
                </TableCell>
                <TableCell align="right">{product.name}</TableCell>
                <TableCell align="right">{product.quantityInStock}</TableCell>
                <TableCell align="right">
                  <Button disabled={product.quantityInStock <= 0} onClick={() => handleOpenDialog(product.id, product.name, product.quantityInStock)}>
                    Encomendar
                  </Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      {selectedProduct && (
        <CreateOrderFormDialog
          open={open}
          onClose={handleCloseDialog}
          product={selectedProduct.name}
          maxQuantity={selectedProduct.quantity}
        />
      )}
    </>
  );
}
