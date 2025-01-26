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

function createData(id: number, name: string, quantity: number) {
  return { id, name, quantity };
}

const rows = [
  createData(1, 'Product 1', 100),
  createData(2, 'Product 2', 100),
  createData(3, 'Product 3', 0), 
];

export default function ProductsComponent() {
  const [open, setOpen] = React.useState(false);
  const [selectedProduct, setSelectedProduct] = React.useState<{
    name: string;
    quantity: number;
  } | null>(null);

  const handleOpenDialog = (productName: string, productQuantity: number) => {
    setSelectedProduct({ name: productName, quantity: productQuantity });
    setOpen(true);
  };

  const handleCloseDialog = () => {
    setOpen(false);
    setSelectedProduct(null);
  };

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
            {rows.map((row) => (
              <TableRow key={row.id} sx={{ '&:last-child td, &:last-child th': { border: 0 } }}>
                <TableCell component="th" scope="row">
                  {row.id}
                </TableCell>
                <TableCell align="right">{row.name}</TableCell>
                <TableCell align="right">{row.quantity}</TableCell>
                <TableCell align="right">
                  <Button
                    disabled={row.quantity <= 0}
                    onClick={() => handleOpenDialog(row.name, row.quantity)}
                  >
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
