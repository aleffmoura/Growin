import * as React from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
interface CreateOrderFormDialogProps {
    open: boolean;
    onClose: () => void;
    product: string;
    maxQuantity: number;
  }
  
  export default function CreateOrderFormDialog({
    open,
    onClose,
    product,
    maxQuantity,
  }: CreateOrderFormDialogProps) {
    const [quantity, setQuantity] = React.useState<number>(1);
  
    const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
      event.preventDefault();
      console.log(`Produto: ${product}, Quantidade: ${quantity}`);
      onClose();
    };
  
    const handleQuantityChange = (event: React.ChangeEvent<HTMLInputElement>) => {
      const value = Math.min(Number(event.target.value), maxQuantity);
      setQuantity(value < 1 ? 1 : value);
    };
  
    return (
      <Dialog
        open={open}
        onClose={onClose}
        PaperProps={{
          component: 'form',
          onSubmit: handleSubmit,
        }}
      >
        <DialogTitle>Encomendar Produto</DialogTitle>
        <DialogContent>
          <DialogContentText>
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
          <Button onClick={onClose}>Cancelar</Button>
          <Button type="submit" disabled={quantity < 1 || quantity > maxQuantity}>
            Confirmar
          </Button>
        </DialogActions>
      </Dialog>
    );
  }
  