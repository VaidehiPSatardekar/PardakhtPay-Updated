import { GenericHelper } from '../helpers/generic';

export interface ISuspension {
  id: number;
  reason: string;
  createdByAccountId: string;
  user: string;
  start: Date;
  end?: Date;
  created: Date;
  active: boolean;
}

export class UserSuspensionForm {
  reason: string;
  time: number;
  unitOfTime: 'hours' | 'days' | 'weeks' | 'months';
  block: boolean;

  constructor(data?: any) {
    GenericHelper.populateData(this, data);
  }
}
