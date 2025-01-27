import * as React from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import Alert from '@mui/material/Alert';
import { createOrder } from '@/app/utils/api';

export default function CreateOrderFormDialog({
  open,
  onClose,
  product,
  productId,
  maxQuantity,
}: CreateOrderDto) {
  const [quantity, setQuantity] = React.useState<number>(1);
  const [loading, setLoading] = React.useState(false);
  const [errors, setErrors] = React.useState<ApiError[]>([]);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setLoading(true);
    setErrors([]);

    const result = await createOrder(productId, quantity);

    if (result.success) {
      onClose(true);
    } else if (result.errors) {
      setErrors(result.errors);
    }

    setLoading(false);
  };

  const handleQuantityChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const value = Math.min(Number(event.target.value), maxQuantity);
    setQuantity(value < 1 ? 1 : value);
  };

  return (
    <Dialog
      open={open}
      onClose={() => onClose(false)}
      slotProps={{
        paper: {
          component: 'form',
          onSubmit: handleSubmit,
        },
      }}>
      <DialogTitle>Encomendar Produto</DialogTitle>
      <DialogContent>
        <DialogContentText>
          Id: <strong>{productId}</strong>.
          <br />
          Faça a encomenda do produto: <strong>{product}</strong>.
          <br />
          Estoque disponível: <strong>{maxQuantity}</strong>.
        </DialogContentText>
        <TextField
          autoFocus
          required
          margin="dense"
          id="quantity"
          name="quantity"
          label="Quantidade"
          type="number"
          fullWidth
          variant="standard"
          inputProps={{
            min: 1,
            max: maxQuantity,
          }}
          value={quantity}
          onChange={handleQuantityChange}
        />
      </DialogContent>
      <DialogActions>
        <Button onClick={() => onClose(false)} disabled={loading}> Cancelar </Button>
        <Button type="submit" disabled={quantity < 1 || quantity > maxQuantity || loading}>
          {loading ? 'Enviando...' : 'Confirmar'}
        </Button>
      </DialogActions>
      {errors.length > 0 && (
        <>
          {errors.map((err, index) => (
            <Alert key={index} severity="error">
              {`${err.propertyName} - ${err.errorMessage}`}
            </Alert>
          ))}
        </>
      )}
    </Dialog>
  );
}
