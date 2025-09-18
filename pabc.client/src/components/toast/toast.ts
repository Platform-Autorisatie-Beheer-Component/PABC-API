import { reactive } from "vue";

type MessageType = "confirm" | "error";

export interface ToastParams {
  text: string;
  type?: MessageType;
  timeout?: number;
  dismissible?: boolean;
}

interface Message {
  text: string;
  type: MessageType;
}

const _messages = reactive<Message[]>([]);

export const messages = _messages as readonly Message[];

export default {
  add(params: ToastParams) {
    const m = {
      text: params.text,
      type: params.type || "confirm"
    };

    _messages.push(m);

    const defaultTimeout = params.type === "error" ? 30000 : 2000;

    setTimeout(() => this.remove(m), params.timeout || defaultTimeout);
  },
  remove(m: Message) {
    const index = _messages.indexOf(m);
    if (index !== -1) _messages.splice(index, 1);
  }
};
