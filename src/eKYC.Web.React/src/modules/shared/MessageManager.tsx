import { Alert, Snackbar } from '@mui/material';
import { useAppDispatch, useAppSelector } from '../../store/hooks';
import { dismissMessage } from '../../store/message';

/** Shows the oldest queued message from the store as a snackbar; mounted once in App. */
export default function MessageManager() {
  const dispatch = useAppDispatch();
  const message = useAppSelector((state) => state.message.queue[0]);

  const close = () => {
    if (message) dispatch(dismissMessage(message.id));
  };

  return (
    <Snackbar
      key={message?.id}
      open={Boolean(message)}
      autoHideDuration={5000}
      onClose={(_, reason) => reason !== 'clickaway' && close()}
      anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
    >
      {message ? (
        <Alert onClose={close} severity={message.severity} variant="filled" sx={{ width: '100%' }}>
          {message.text}
        </Alert>
      ) : undefined}
    </Snackbar>
  );
}
