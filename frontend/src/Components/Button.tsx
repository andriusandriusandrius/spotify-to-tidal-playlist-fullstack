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
        'min-w-24 rounded-md p-4 text-sm font-bold',
        variant === 'primary'
          ? disabled
            ? 'bg-slate-200 text-white'
            : 'bg-slate-400 text-white hover:bg-slate-300'
          : disabled
            ? 'border border-slate-200 text-gray-500'
            : 'border border-slate-400 text-black',
        disabled ? 'cursor-not-allowed' : 'cursor-pointer',
      )}
      onClick={onClick}
      disabled={disabled}
    >
      {label}
    </button>
  );
}
