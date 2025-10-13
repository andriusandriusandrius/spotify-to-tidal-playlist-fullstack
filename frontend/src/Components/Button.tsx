import { clsx } from 'clsx';

type ButtonProps = {
  type: 'submit' | 'button';
  label: string;
  variant: 'primary' | 'secondary';
  onClick?: () => void;
  disabled?: boolean;
};
export function Button({ type, label, variant, onClick, disabled }: ButtonProps) {
  return (
    <button
      type={type}
      className={clsx(
        'min-w-24 rounded-md border-4 p-4 text-sm font-bold',
        variant === 'primary'
          ? disabled
            ? 'bg-light-brown text-white'
            : 'bg-dark-brown text-dark-blue hover:bg-light-blue'
          : disabled
            ? 'border border-light-brown text-white'
            : 'border border-light-brown text-dark-brown hover:border-light-brown hover:bg-light-blue',
        disabled ? 'cursor-not-allowed' : 'cursor-pointer',
      )}
      onClick={onClick}
      disabled={disabled}
    >
      {label}
    </button>
  );
}
